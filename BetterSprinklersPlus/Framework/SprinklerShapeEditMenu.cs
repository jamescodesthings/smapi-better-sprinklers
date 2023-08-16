using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace BetterSprinklersPlus.Framework
{
  internal enum TileState
  {
    Off = 0,
    On = 1,
    DefaultTile = 2
  }

  internal class SprinklerShapeEditMenu : IClickableMenu
  {
    private readonly IModHelper Helper;
    private readonly IMonitor Monitor;
    private readonly Texture2D WhitePixel;
    private readonly List<ClickableComponent> Tabs;
    private readonly ClickableTextureComponent OkButton;

    private const int MaxArraySize = 15;
    private const int DefaultTileSize = 32;
    private const int Padding = 2;
    private const int MinLeftMargin = 32;
    private const int MinTopMargin = 32;
    private const int TabDistanceFromMenu = -32;
    private const int TabItemWidth = 64;
    private const int TabItemHeight = 64;
    private const int TabDistanceVerticalBetweenTabs = 16;
    private const int TabLeftMargin = 16;
    private const int TabVerticalMargins = 16;
    private const int TabRightMargin = 32;

    private int ArraySize = 15;
    private int CenterTile = 7;
    private int TileSize = 32;

    private int HoveredItemX;
    private int HoveredItemY;

    private readonly Color[] Colors = { Color.Tomato, Color.ForestGreen, Color.LightSteelBlue };

    private int LeftMargin;
    private int TopMargin;

    private int[,] SprinklerGrid;

    private int ActiveSprinklerSheet;

    private TileState? Toggling;

    private int Cost = 0;


    /// <summary>Constructor</summary>
    public SprinklerShapeEditMenu(IModHelper helper, IMonitor monitor)
    {
      Helper = helper;
      Monitor = monitor;
      const int menuWidth = MaxArraySize * DefaultTileSize + MinLeftMargin * 2;
      const int menuHeight = MaxArraySize * DefaultTileSize + MinTopMargin * 2;
      var menuX = Game1.viewport.Width / 2 - menuWidth / 2;
      var menuY = Game1.viewport.Height / 2 - menuHeight / 2;
      initialize(menuX, menuY, menuWidth, menuHeight, true);

      Tabs = new List<ClickableComponent>();
      const int tabWidth = TabItemWidth + TabLeftMargin + TabRightMargin;
      const int tabHeight = TabItemHeight + TabVerticalMargins * 2;
      Tabs.Add(new ClickableComponent(
        new Rectangle(menuX - TabDistanceFromMenu - tabWidth,
          menuY + TabDistanceVerticalBetweenTabs, tabWidth, tabHeight),
        new Object(Vector2.Zero, 599)));
      Tabs.Add(new ClickableComponent(
        new Rectangle(menuX - TabDistanceFromMenu - tabWidth,
          Tabs[0].bounds.Y + tabHeight + TabDistanceVerticalBetweenTabs, tabWidth, tabHeight),
        new Object(Vector2.Zero, 621)));
      Tabs.Add(new ClickableComponent(
        new Rectangle(menuX - TabDistanceFromMenu - tabWidth,
          Tabs[1].bounds.Y + tabHeight + TabDistanceVerticalBetweenTabs, tabWidth, tabHeight),
        new Object(Vector2.Zero, 645)));

      OkButton = new ClickableTextureComponent("save-changes",
        new Rectangle(xPositionOnScreen + width - Game1.tileSize / 2,
          yPositionOnScreen + height - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize), "",
        "Save Changes", Game1.mouseCursors, new Rectangle(128, 256, 64, 64), 1f);

      WhitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
      WhitePixel.SetData(new[] { Color.White });

      SetActiveSprinklerSheetIndex(599);
    }

    public override void draw(SpriteBatch b)
    {
      foreach (var tab in Tabs)
      {
        drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), tab.bounds.X,
          tab.bounds.Y, tab.bounds.Width, tab.bounds.Height, Color.White);
        Game1.spriteBatch.Draw(Game1.objectSpriteSheet,
          new Rectangle(tab.bounds.X + TabLeftMargin, tab.bounds.Y + TabVerticalMargins, TabItemWidth,
            TabItemHeight),
          Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, tab.item.ParentSheetIndex, 16, 16),
          Color.White);
      }

      drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
        xPositionOnScreen, yPositionOnScreen, width, height, Color.White);

      // Draw Cost
      var font = Game1.smallFont;
      Utility.drawTextWithShadow(b, $"{Cost}G", font, new Vector2(xPositionOnScreen + 20, (yPositionOnScreen + width) - 100), Game1.textColor);


      var countX = 0;
      int x;
      int y;

      if (HoveredItemX > -1 && HoveredItemY > -1)
      {
        x = xPositionOnScreen + LeftMargin + HoveredItemX * TileSize;
        y = yPositionOnScreen + TopMargin + HoveredItemY * TileSize;
        Game1.spriteBatch.Draw(WhitePixel, new Rectangle(x, y, TileSize, TileSize), Color.AntiqueWhite);
      }

      while (countX < ArraySize)
      {
        var countY = 0;
        while (countY < ArraySize)
        {
          x = xPositionOnScreen + LeftMargin + Padding + countX * TileSize;
          y = yPositionOnScreen + TopMargin + Padding + countY * TileSize;
          Game1.spriteBatch.Draw(WhitePixel,
            new Rectangle(x, y, TileSize - Padding * 2, TileSize - Padding * 2),
            Colors[SprinklerGrid[countX, countY]]);
          ++countY;
        }

        ++countX;
      }

      x = xPositionOnScreen + LeftMargin + Padding + CenterTile * TileSize;
      y = yPositionOnScreen + TopMargin + Padding + CenterTile * TileSize;
      Game1.spriteBatch.Draw(Game1.objectSpriteSheet,
        new Rectangle(x, y, TileSize - Padding * 2, TileSize - Padding * 2),
        Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ActiveSprinklerSheet, 16, 16),
        Color.White);
      OkButton.draw(Game1.spriteBatch);

      Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
        Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero,
        4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0);

      base.draw(b);
    }

    public override void update(GameTime time)
    {
      base.update(time);

      var mouseGridRelX = Game1.getOldMouseX() - xPositionOnScreen - LeftMargin - Padding;
      var mouseGridRelY = Game1.getOldMouseY() - yPositionOnScreen - TopMargin - Padding;

      if (mouseGridRelX > 0 && mouseGridRelY > 0 && mouseGridRelX < ArraySize * TileSize - Padding &&
          mouseGridRelY < ArraySize * TileSize - Padding)
      {
        HoveredItemX = mouseGridRelX / TileSize;
        HoveredItemY = mouseGridRelY / TileSize;
      }
      else
      {
        HoveredItemX = -1;
        HoveredItemY = -1;
      }

      // here, if mouse is down and hoveredItem is a tile, toggle the tile.
      var isLeftMousePressed = Helper.Input.IsDown(SButton.MouseLeft);

      if (!isLeftMousePressed)
      {
        ResetToggle();
      }

      if (isLeftMousePressed && HoveredItemX != -1 && HoveredItemY != -1)
      {
        Toggle();
      }


      OkButton.tryHover(Game1.getOldMouseX(), Game1.getOldMouseY());
    }

    private void Toggle()
    {
      if (SprinklerGrid[HoveredItemX, HoveredItemY] == (int)TileState.DefaultTile) return;

      if (Toggling == null)
      {
        var current = (TileState)SprinklerGrid[HoveredItemX, HoveredItemY];
        Toggling = current == TileState.Off ? TileState.On : TileState.Off;
      }

      SprinklerGrid[HoveredItemX, HoveredItemY] = (int)Toggling;
      RecalculateCost();
    }


    private void ResetToggle()
    {
      Toggling = null;
    }


    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, playSound);

      var mouseGridRelX = x - xPositionOnScreen - LeftMargin - Padding;
      var mouseGridRelY = y - yPositionOnScreen - TopMargin - Padding;

      if (mouseGridRelX > 0 && mouseGridRelY > 0 && mouseGridRelX < ArraySize * TileSize - Padding &&
          mouseGridRelY < ArraySize * TileSize - Padding)
      {
        HoveredItemX = mouseGridRelX / TileSize;
        HoveredItemY = mouseGridRelY / TileSize;
        if (SprinklerGrid[HoveredItemX, HoveredItemY] != 2)
        {
          SprinklerGrid[HoveredItemX, HoveredItemY] =
            1 - SprinklerGrid[HoveredItemX, HoveredItemY];
          Game1.playSound("select");
          RecalculateCost();
        }
      }
      else
      {
        HoveredItemX = -1;
        HoveredItemY = -1;

        foreach (var tab in Tabs)
        {
          if (tab.containsPoint(x, y))
          {
            Game1.playSound("select");
            SetActiveSprinklerSheetIndex(tab.item.ParentSheetIndex);
          }
        }

        if (OkButton.containsPoint(x, y))
        {
          Game1.playSound("select");
          BetterSprinklersPlusConfig.SaveChanges();
        }
      }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }


    /*********
     ** Private methods
     *********/
    private void SetActiveSprinklerSheetIndex(int type)
    {
      ActiveSprinklerSheet = type;

      HoveredItemX = -1;
      HoveredItemY = -1;

      SprinklerGrid = BetterSprinklersPlusConfig.Active.SprinklerShapes[type];

      switch (type)
      {
        case 599:
          ArraySize = 7;
          CenterTile = ArraySize / 2;
          TileSize = 64;

          SprinklerGrid[CenterTile, CenterTile] = 2;
          SprinklerGrid[CenterTile - 1, CenterTile] = 2;
          SprinklerGrid[CenterTile + 1, CenterTile] = 2;
          SprinklerGrid[CenterTile, CenterTile - 1] = 2;
          SprinklerGrid[CenterTile, CenterTile + 1] = 2;
          break;
        case 621:
          TileSize = 32;
          ArraySize = 11;
          CenterTile = ArraySize / 2;

          for (var x = CenterTile - 1; x < CenterTile + 2; x++)
          {
            for (var y = CenterTile - 1; y < CenterTile + 2; y++)
              SprinklerGrid[x, y] = 2;
          }

          break;
        case 645:
          TileSize = 32;
          ArraySize = 15;
          CenterTile = ArraySize / 2;

          for (var x = CenterTile - 2; x < CenterTile + 3; x++)
          {
            for (var y = CenterTile - 2; y < CenterTile + 3; y++)
              SprinklerGrid[x, y] = 2;
          }

          break;
      }

      LeftMargin = (width - (ArraySize * TileSize)) / 2;
      TopMargin = (height - (ArraySize * TileSize)) / 2;

      RecalculateCost();
    }

    private void RecalculateCost()
    {
      var wateredTileCount = GetCountOfTilesBeingWatered();
      Cost = CalculateCost(wateredTileCount);
    }

    private int GetCountOfTilesBeingWatered()
    {
      var count = 0;
      for (var x = 0; x < ArraySize; x++)
      {
        for (var y = 0; y < ArraySize; y++)
        {
          if (SprinklerGrid[x, y] != (int)TileState.Off)
          {
            count++;
          }
        }
      }

      return count;
    }

    private int CalculateCost(int count)
    {
      var costPerTile = GetCostPerTile();
      var cost = (int)Math.Round(count * costPerTile);
      return cost;
    }

    /// <summary>
    /// Gets the cost per tile in .Gs
    /// </summary>
    /// <returns>The cost of watering one tile (as a fraction of a G)</returns>
    private float GetCostPerTile()
    {
      try
      {
        return BetterSprinklersPlusConfig.BalancedModeOptionsMultipliers[BetterSprinklersPlusConfig.Active.BalancedMode];
      }
      catch (Exception e)
      {
        Monitor.Log($"Error getting cost per tile {e.Message}", LogLevel.Error);
        return 0f;
      }
    }
  }
}
