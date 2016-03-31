/*
    Copyright 2016 Maurício Gomes (Speeder)

    Configurable Improved Sprinklers mod is free software: 
    you can redistribute it and/or modify it under the terms of the 
    GNU General Public License as published by the Free Software Foundation,
    either version 3 of the License, or (at your option) any later version.

    Configurable Improved Sprinklers mod is distributed in the hope
    that it will be useful, but WITHOUT ANY WARRANTY; 
    without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Configurable Improved Sprinklers mod. 
    If not, see <http://www.gnu.org/licenses/>.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SMAPISprinklerMod;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Inheritance;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace SprinklerMod
{
    public class SprinklerMod : Mod
    {
        public static SprinklerModConfig ModConfig { get; protected set; }

        int[] validSprinklers;

        private static Dictionary<string, string> oldCraftingRecipes;
        private static Dictionary<int, string> oldObjectInfo;

        public override void Entry(params object[] objects)
        {
            ModConfig = new SprinklerModConfig().InitializeConfig(BaseConfigPath);
            oldCraftingRecipes = null;
            oldObjectInfo = null;

            TimeEvents.DayOfMonthChanged += Event_ChangedDayOfMonth;
            GameEvents.LoadContent += Event_LoadContent;
            GameEvents.UpdateTick += Event_UpdateTick;                            
        }

        static void Event_ChangedDayOfMonth(object sender, EventArgs e)
        {                      
            foreach(GameLocation location in Game1.locations)
            {
                foreach (KeyValuePair<Vector2, StardewValley.Object> objectPair in location.objects)
                {
                    StardewValley.Object obj = objectPair.Value;
                    Vector2 centerLocation = objectPair.Key;                    
                    if (ModConfig.sprinklerShapes.ContainsKey(obj.parentSheetIndex))
                    {
                        int[,] configGrid = ModConfig.sprinklerShapes[obj.parentSheetIndex];

                        Vector2 iterativeLocation = centerLocation;
                        int arrayHalfSizeX = ModConfig.sprinklerShapes[obj.parentSheetIndex].GetLength(0) / 2;
                        int arrayHalfSizeY = ModConfig.sprinklerShapes[obj.parentSheetIndex].GetLength(1) / 2;
                        iterativeLocation.X -= arrayHalfSizeX;
                        iterativeLocation.Y -= arrayHalfSizeY;
                        float maxX = centerLocation.X + arrayHalfSizeX + 1;
                        float maxY = centerLocation.Y + arrayHalfSizeY + 1;

                        int counterX = 0;
                        int counterY = 0;

                        while(iterativeLocation.X < maxX)
                        {
                            iterativeLocation.Y = centerLocation.Y - arrayHalfSizeY;
                            counterY = 0;
                            while(iterativeLocation.Y < maxY)
                            {                            
                                if (configGrid[counterX, counterY] > 0 && location.terrainFeatures.ContainsKey(iterativeLocation))
                                {
                                    if (location.terrainFeatures[iterativeLocation] is HoeDirt)
                                    {
                                        (location.terrainFeatures[iterativeLocation] as HoeDirt).state = 1;
                                    }
                                }
                                ++iterativeLocation.Y;
                                ++counterY;
                            }
                            ++iterativeLocation.X;
                            ++counterX;
                        }
                    }
                }
            }
        }

        static void Event_LoadContent(object sender, EventArgs e)
        {
            UpdatePrices();       
        }

        static void Event_UpdateTick(object sender, EventArgs e)
        {
            if(Game1.activeClickableMenu == null && Game1.CurrentEvent == null)
            {
                KeyboardState currentKeyboardState = Keyboard.GetState();
                if (currentKeyboardState.IsKeyDown(Keys.K))
                {                    
                    Game1.activeClickableMenu = new SprinklerShapeEditMenu();
                }
            }
        }        

        public static void UpdatePrices()
        {
            string[] infoSplit;
            string[] ingredientsSplit;
            int counter;            

            if(oldCraftingRecipes == null)
            {
                oldCraftingRecipes = CraftingRecipe.craftingRecipes;
                oldObjectInfo = Game1.objectInformation;           
            }
            else
            {
                CraftingRecipe.craftingRecipes = oldCraftingRecipes;
                Game1.objectInformation = oldObjectInfo;
            }
            

            Dictionary<string, string> newCraftingRecipes = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> craftingRecipe in CraftingRecipe.craftingRecipes)
            {                
                if (craftingRecipe.Key.Contains("prinkler"))
                {
                    Log.Debug(String.Format("key {0} value {1}", craftingRecipe.Key, craftingRecipe.Value));
                    infoSplit = craftingRecipe.Value.Split('/');
                    int sprinklerSheet = infoSplit[2].AsInt32();
                    int multiplier = ModConfig.sprinklerPrices[sprinklerSheet];
                    ingredientsSplit = infoSplit[0].Split(' ');
                    counter = 1;
                    while (counter < ingredientsSplit.Length)
                    {
                        ingredientsSplit[counter] = (ingredientsSplit[counter].AsInt32() * multiplier).ToString();
                        counter += 2;
                    }
                    infoSplit[0] = string.Join(" ", ingredientsSplit);
                    newCraftingRecipes[craftingRecipe.Key] = string.Join("/", infoSplit);
                    Log.Debug(String.Format("key {0} value {1}", craftingRecipe.Key, newCraftingRecipes[craftingRecipe.Key]));
                }
                else
                {
                    newCraftingRecipes[craftingRecipe.Key] = craftingRecipe.Value;
                }                
            }

            Dictionary<int, string> newObjectInfo = new Dictionary<int, string>();
            foreach (KeyValuePair<int, string> objectInfo in Game1.objectInformation)
            {
                if (ModConfig.sprinklerPrices.ContainsKey(objectInfo.Key))
                {
                    int multiplier = ModConfig.sprinklerPrices[objectInfo.Key];
                    infoSplit = objectInfo.Value.Split('/');
                    Log.Debug(String.Format("object index {0}, name {1}, old price {2}, new price {3}", objectInfo.Key, infoSplit[0], infoSplit[1], infoSplit[1].AsInt32() * multiplier));
                    infoSplit[1] = (infoSplit[1].AsInt32() * multiplier).ToString();
                    newObjectInfo[objectInfo.Key] = string.Join("/", infoSplit);
                }
                else
                {
                    newObjectInfo[objectInfo.Key] = objectInfo.Value;
                }
            }

            CraftingRecipe.craftingRecipes = newCraftingRecipes;
            Game1.objectInformation = newObjectInfo;
        }
    }
}
