using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Inheritance;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerMod
{
    public class SprinklerMod : Mod
    {
        public override string Name
        {
            get { return "Sprinkler Mod"; }
        }

        public override string Authour
        {
            get { return "Maurício Gomes (Speeder)"; }
        }

        public override string Version
        {
            get { return "1.0"; }
        }

        public override string Description
        {
            get { return "Make the Sprinklers work better."; }
        }

        public override void Entry(params object[] objects)
        {
            TimeEvents.DayOfMonthChanged += Event_ChangedDayOfMonth;            
        }

        static void Event_ChangedDayOfMonth(object sender, EventArgs e)
        {
            GameLocation farm = SGame.getLocationFromName("Farm");
            foreach (StardewValley.Object obj in farm.objects.Values)
            {
                if (obj.parentSheetIndex == 599)
                {
                    Vector2 location = obj.tileLocation;

                    if (farm.terrainFeatures.ContainsKey(location) && farm.terrainFeatures[location] is HoeDirt)
                    {
                        (farm.terrainFeatures[location] as HoeDirt).state = 1;
                    }

                    location.X -= 2;

                    if (farm.terrainFeatures.ContainsKey(location) && farm.terrainFeatures[location] is HoeDirt)
                    {
                        (farm.terrainFeatures[location] as HoeDirt).state = 1;
                    }

                    location.X += 4;
                    if (farm.terrainFeatures.ContainsKey(location) && farm.terrainFeatures[location] is HoeDirt)
                    {
                        (farm.terrainFeatures[location] as HoeDirt).state = 1;
                    }

                    location.X -= 2;
                    location.Y -= 2;
                    if (farm.terrainFeatures.ContainsKey(location) && farm.terrainFeatures[location] is HoeDirt)
                    {
                        (farm.terrainFeatures[location] as HoeDirt).state = 1;
                    }

                    location.Y += 4;
                    if (farm.terrainFeatures.ContainsKey(location) && farm.terrainFeatures[location] is HoeDirt)
                    {
                        (farm.terrainFeatures[location] as HoeDirt).state = 1;
                    }
                }
                else if (obj.parentSheetIndex == 621)
                {
                    Vector2 columnLocation = obj.TileLocation;
                    Vector2 rowLocation = obj.TileLocation;
                    float maxLocationY = columnLocation.Y + 4;
                    float maxLocationX = rowLocation.X + 4;
                    columnLocation.Y -= 3;
                    rowLocation.X -= 3;

                    while(columnLocation.Y < maxLocationY)
                    {
                        if (farm.terrainFeatures.ContainsKey(columnLocation) && farm.terrainFeatures[columnLocation] is HoeDirt)
                        {
                            (farm.terrainFeatures[columnLocation] as HoeDirt).state = 1;
                        }
                        ++columnLocation.Y;
                    }

                    while (rowLocation.X < maxLocationX)
                    {
                        if (farm.terrainFeatures.ContainsKey(rowLocation) && farm.terrainFeatures[rowLocation] is HoeDirt)
                        {
                            (farm.terrainFeatures[rowLocation] as HoeDirt).state = 1;
                        }
                        ++rowLocation.X;
                    }
                }
                else if (obj.parentSheetIndex == 645)
                {
                    Vector2 columnLocation = obj.TileLocation;
                    Vector2 rowLocation = obj.TileLocation;
                    float maxLocationY = columnLocation.Y + 7;
                    float maxLocationX = rowLocation.X + 7;
                    columnLocation.Y -= 6;
                    rowLocation.X -= 6;

                    while (columnLocation.Y < maxLocationY)
                    {
                        if (farm.terrainFeatures.ContainsKey(columnLocation) && farm.terrainFeatures[columnLocation] is HoeDirt)
                        {
                            (farm.terrainFeatures[columnLocation] as HoeDirt).state = 1;
                        }
                        ++columnLocation.Y;
                    }

                    while (rowLocation.X < maxLocationX)
                    {
                        if (farm.terrainFeatures.ContainsKey(rowLocation) && farm.terrainFeatures[rowLocation] is HoeDirt)
                        {
                            (farm.terrainFeatures[rowLocation] as HoeDirt).state = 1;
                        }
                        ++rowLocation.X;
                    }
                }
            }
        }
    }
}
