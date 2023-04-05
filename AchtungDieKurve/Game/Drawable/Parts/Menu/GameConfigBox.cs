using System;
using System.Collections.Generic;
using System.Linq;
using AchtungDieKurve.Game.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace AchtungDieKurve.Game.Drawable.Parts.Menu
{
    class GameConfigBox : DefaultDrawable, IContainer
    {
        public Rectangle ContentArea { get; private set; }

        private List<PlayerDefinition> _playerDefinitions;
        private List<Keys> _forbiddenControls = new List<Keys>
        {
            Keys.F1,Keys.F2,Keys.F3,Keys.F4,Keys.F5,Keys.F6,Keys.F7,Keys.F8,Keys.F9,Keys.F10,Keys.F11,Keys.F12,
            Keys.CapsLock, Keys.Scroll, Keys.PrintScreen, Keys.Escape, Keys.Enter, Keys.NumLock, Keys.LeftWindows, Keys.RightWindows,
            Keys.Back, Keys.Delete
        }; 
        private readonly GameScreen _screen;
        public static int MarginHorizontal;
        public const int YOffset = 305;
        public const int MarginTop = 50;
        public const int MarginBottom = 50;
        public const int HeaderTextY = 12;
        public const int NameColX = 12;
        public const int ReadyColX = -70;
        public const int KeysColX = -380;
        public const int TypeColX = -480;
        public const int PlayerRowSpacing = 40;

        private int _selectedIdx = 0;
        private int _editingField = 0;
        private bool _editingName;
        private bool _editingKeys;
        private bool _editingType;
        private string _editingNameTemp;
        private bool _editingTypeTemp;
        private Tuple<Keys, Keys> _editingKeysTemp;
        private int _editKeysRound;
        private Keys _lastWrittenKey;

        private double _colorTransition;

        public int SelectedId
        {
            get { return _selectedIdx; }
            set { _selectedIdx = (int) MathHelper.Clamp(value, 0, _playerDefinitions.Count -1); }
        }

        public bool Active
        {
            get { return _active; }
            set
            {
                if (value)
                {
                    _selectedIdx = 0;
                }
                _active = value;
            }
        }

        private bool _active;

        public GameConfigBox(GameBase game, SpriteBatch spriteBatch, ref List<PlayerDefinition> playerDefinitions, GameScreen screen)
            :base(game, spriteBatch)
        {
            _playerDefinitions = playerDefinitions;
            _screen = screen;
            ContentArea = CreateContainer();
        }

        private Rectangle CreateContainer()
        {
            MarginHorizontal = GameBase.Settings.ScreenWidth / 18;
            return new Rectangle(
                GameConfigBox.MarginHorizontal,
                GameConfigBox.MarginTop + YOffset,
                GameBase.Settings.ScreenWidth - (2*GameConfigBox.MarginHorizontal),
                GameBase.Settings.ScreenHeight - YOffset - GameConfigBox.MarginTop - GameConfigBox.MarginBottom
                );
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (_active)
            {
                spriteBatch.Draw(CommonResources.whitepixel, new Rectangle(0,0, GameBase.Settings.ScreenWidth, GameBase.Settings.ScreenHeight), CommonResources.TransparentOverlayColor);
                spriteBatch.Draw(CommonResources.whitepixel, GetHelpRectangleBorder(), CommonResources.TableBodyColor);
                spriteBatch.Draw(CommonResources.whitepixel, GetHelpRectangle(), CommonResources.TransparentOverlayColor);
                DrawHelp(GetHelpRectangle());
            }
            spriteBatch.Draw(CommonResources.whitepixel, GetBorderRectangle(), CommonResources.TableBodyColor);
            spriteBatch.Draw(CommonResources.whitepixel, GetContentRectangle(), Color.Black);
            DrawHeader();
            DrawPlayers(ContentArea);
        }

        private void DrawHeader()
        {
            var header = new Rectangle(
                ContentArea.X,
                ContentArea.Y - 40,
                ContentArea.Width,
                40
                );
            spriteBatch.Draw(CommonResources.whitepixel, header, CommonResources.TableBodyColor);
            spriteBatch.DrawString(CommonResources.fontSmallBold, "Player name", new Vector2(header.X + NameColX, header.Y + HeaderTextY), CommonResources.Borders);
            spriteBatch.DrawString(CommonResources.fontSmallBold, "Type", new Vector2(header.Right + TypeColX, header.Y + HeaderTextY), CommonResources.Borders);
            spriteBatch.DrawString(CommonResources.fontSmallBold, "Keys", new Vector2(header.Right + KeysColX, header.Y + HeaderTextY), CommonResources.Borders);
            spriteBatch.DrawString(CommonResources.fontSmallBold, "Ready", new Vector2(header.Right + ReadyColX, header.Y + HeaderTextY), CommonResources.Borders);
        }

        private void DrawPlayers(Rectangle container)
        {
            
            var y = container.Y + 16;
            for (var i = 0; i < _playerDefinitions.Count; i++)
            {
                var p = _playerDefinitions[i];
                var nameColor = p.Color;
                // active row
                if (_active && i == _selectedIdx)
                {
                    spriteBatch.Draw(CommonResources.whitepixel, new Rectangle(
                        container.X, y - 7, container.Width, 40
                        ), CommonResources.Background );
                }

                // drawing currently editing player name
                if (_editingName && SelectedId == i)
                {
                    var measure = CommonResources.fontMedium.MeasureString(_editingNameTemp);
                    spriteBatch.DrawString(CommonResources.fontMedium, _editingNameTemp + "_", new Vector2(container.X + NameColX, y),
                        nameColor);
                    spriteBatch.DrawString(CommonResources.fontMedium, "_", new Vector2(container.X + NameColX + measure.X, y),
                        Color.White);
                }
                    // drawing selected name column 
                else if (SelectedId == i && _editingField == 0 && !_editingName && Active)
                {
                    var measureParenthesis = CommonResources.fontMedium.MeasureString("[");
                    var measure = CommonResources.fontMedium.MeasureString(p.Name);
                    spriteBatch.DrawString(CommonResources.fontMedium, "[", new Vector2(container.X + NameColX, y),
                        CreateColorTransition(nameColor));
                    spriteBatch.DrawString(CommonResources.fontMedium, p.Name,
                        new Vector2(container.X + NameColX + measureParenthesis.X + 5, y), nameColor);
                    spriteBatch.DrawString(CommonResources.fontMedium, "]",
                        new Vector2(container.X + NameColX + measureParenthesis.X + measure.X + 10, y), CreateColorTransition(nameColor));
                }
                    // drawing name normally 
                else
                {
                    spriteBatch.DrawString(CommonResources.fontMedium, p.Name,
                        new Vector2(container.X + NameColX, y), nameColor);
                }

                string playerTypeString = _playerDefinitions[i].IsAi ? "Computer" : "Human";

                // drawing currently selected player type column
                if (_editingField == 1 && SelectedId == i && _editingType == false)
                {
                    var measureParenthesis = CommonResources.fontSmall.MeasureString("[");
                    var measure = CommonResources.fontSmall.MeasureString(playerTypeString);
                    spriteBatch.DrawString(CommonResources.fontSmall, "[", new Vector2(container.Right + TypeColX, y),
                        CreateColorTransition(CommonResources.Borders));
                    spriteBatch.DrawString(CommonResources.fontSmall, playerTypeString,
                        new Vector2(container.Right + TypeColX + measureParenthesis.X + 5, y), nameColor);
                    spriteBatch.DrawString(CommonResources.fontSmall, "]",
                        new Vector2(container.Right + TypeColX + measureParenthesis.X + measure.X + 10, y),
                        CreateColorTransition(CommonResources.Borders));

                } 
                    // draw editing type
                else if(_editingField == 1 && _editingType && SelectedId == i)
                {
                    var playerTypeStringTemp = _editingTypeTemp ? "Computer" : "Human";
                    spriteBatch.DrawString(CommonResources.fontSmall, playerTypeStringTemp,
                        new Vector2(container.Right + TypeColX, y), nameColor);
                }    
                    //drawing normally player type
                else
                {
                    spriteBatch.DrawString(CommonResources.fontSmall, playerTypeString,
                        new Vector2(container.Right + TypeColX, y), CommonResources.Borders);
                }

                // drawing currently selected keys column
                if (_editingField == 2 && SelectedId == i && _editingKeys == false)
                {
                    var keyString = p.Left + "," + p.Right;
                    var measureParenthesis = CommonResources.fontSmall.MeasureString("[");
                    var measure = CommonResources.fontSmall.MeasureString(keyString);
                    spriteBatch.DrawString(CommonResources.fontSmall, "[", new Vector2(container.Right + KeysColX, y),
                        CreateColorTransition(CommonResources.Borders));
                    spriteBatch.DrawString(CommonResources.fontSmall, keyString,
                        new Vector2(container.Right + KeysColX + measureParenthesis.X + 5, y), nameColor);
                    spriteBatch.DrawString(CommonResources.fontSmall, "]",
                        new Vector2(container.Right + KeysColX + measureParenthesis.X + measure.X + 10, y),
                        CreateColorTransition(CommonResources.Borders));
                }
                    // draw currently editing keys
                else if (_editingField == 2 && SelectedId == i && _editingKeys == true)
                {
                    string key1, key2;
                    Color color1, color2;
                    if (_editKeysRound == 0)
                    {
                        key1 = "<?>";
                        key2 = _editingKeysTemp.Item2.ToString();
                        color1 = nameColor;
                        color2 = CommonResources.Borders;
                    }
                    else
                    {
                        key2 = "<?>";
                        key1 = _editingKeysTemp.Item1.ToString();
                        color2 = nameColor;
                        color1 = CommonResources.Borders;
                    }
                    var measure1 = CommonResources.fontSmall.MeasureString(key1);
                    var measure2 = CommonResources.fontSmall.MeasureString(key2);
                    var measureComma = CommonResources.fontSmall.MeasureString(",");

                    spriteBatch.DrawString(CommonResources.fontSmall, key1, new Vector2(container.Right + KeysColX, y),
                        color1);

                    spriteBatch.DrawString(CommonResources.fontSmall, ",",
                        new Vector2(container.Right + KeysColX + measure1.X + 3, y), CommonResources.Borders);

                    spriteBatch.DrawString(CommonResources.fontSmall, key2,
                        new Vector2(container.Right + KeysColX + measure1.X + measure2.X + 6 + measureComma.X, y),
                        color2);
                }
                    // drawing keys normally
                else
                {
                    spriteBatch.DrawString(CommonResources.fontSmall, p.Left + "," + p.Right, new Vector2(container.Right + KeysColX, y + 4), CommonResources.Borders);    
                }
                
                spriteBatch.Draw(CommonResources.kurveBody, new Rectangle(container.Right + ReadyColX + 14, y + 7, 13 , 13), p.Active ? CommonResources.MainColor:CommonResources.TableBodyColor);
                y += PlayerRowSpacing;
            }
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);
            if (_editingKeys)
            {
                HandleEditKeysInput(input);
                return;
            }
            if (_editingName)
            {
                HandleEditNameInput(input);
                return;
            }

            if (_editingType)
            {
                HandleEditTypeInput(input);
                return;
            }
            
            HandleListInput(input);
        }

        private void HandleListInput(InputState input)
        {
            PlayerIndex player;
            if (input.IsMenuDown(_screen.ControllingPlayer.Value))
            {
                SelectedId++;
                //_editingField = 0;
                _editingKeys = false;
                _editingName = false;
            }
            if (input.IsMenuUp(_screen.ControllingPlayer.Value))
            {
                SelectedId--;
                //_editingField = 0;
                _editingKeys = false;
                _editingName = false;
            }
            if (input.IsMenuCancel(_screen.ControllingPlayer.Value, out player))
            {
                SaveSettings();
                Active = !Active;
                _editingField = 0;
                _editingType = false;
                _editingKeys = false;
                _editingName = false;
                SelectedId = -1;
            }

            if (input.IsNewKeyPress(Keys.A, _screen.ControllingPlayer, out player))
            {
                if (_playerDefinitions[SelectedId].IsAi)
                {
                    _playerDefinitions[SelectedId].IsAi = false;
                    _playerDefinitions[SelectedId].Active = false;
                }
                else
                {
                    _playerDefinitions[SelectedId].IsAi = true;
                    _playerDefinitions[SelectedId].Active = true;
                }
            }
            if (input.IsNewKeyPress(Keys.P, _screen.ControllingPlayer, out player))
            {
                if (GameBase.Settings.ScreenHeight - YOffset - (((_playerDefinitions.Count + 2) * PlayerRowSpacing)) <= 0) { return; }
                _playerDefinitions.Add(new PlayerDefinition("New player", Keys.None, Keys.None, 
                    Color.FromNonPremultiplied(
                        GameBase.Settings.Rand.Next(0,255),
                        GameBase.Settings.Rand.Next(0, 255),
                        GameBase.Settings.Rand.Next(0, 255),
                        255
                        )));
                SelectedId = _playerDefinitions.Count;
            }
            if (input.IsNewKeyPress(Keys.Delete, _screen.ControllingPlayer, out player))
            {
                _playerDefinitions.RemoveAt(SelectedId);
                SelectedId = (int)MathHelper.Clamp(SelectedId - 1, 0, _playerDefinitions.Count);
            }
            if (input.IsNewKeyPress(Keys.Left, _screen.ControllingPlayer, out player))
            {
                _editingField = (int) MathHelper.Clamp(_editingField - 1, 0, 2);
            }
            if (input.IsNewKeyPress(Keys.Right, _screen.ControllingPlayer, out player))
            {
                _editingField = (int)MathHelper.Clamp(_editingField + 1, 0, 2);
            }
            if (input.IsNewKeyPress(Keys.Enter, _screen.ControllingPlayer, out player))
            {
                switch (_editingField)
                {
                    case 0:
                        _editingNameTemp = _playerDefinitions[SelectedId].Name;
                        _editingName = true;
                        break;
                    case 1:
                        _editingTypeTemp = _playerDefinitions[SelectedId].IsAi;
                        _editingType = true;
                        break;
                    case 2:
                        _editingKeysTemp = new Tuple<Keys, Keys>(_playerDefinitions[SelectedId].Left, _playerDefinitions[SelectedId].Right);
                        _editingKeys = true;
                        _editKeysRound = 0;
                        break;
                }
            }
            
        }

        private void HandleEditNameInput(InputState input)
        {
            PlayerIndex player;

            if (input.IsNewKeyPress(Keys.Enter, _screen.ControllingPlayer, out player))
            {
                _playerDefinitions[SelectedId].Name = _editingNameTemp;
                SaveSettings();
                _editingName = false;
            }
            if (input.IsNewKeyPress(Keys.Escape, _screen.ControllingPlayer, out player))
            {
                _editingName = false;
            }

            // deleting
            if (input.IsNewKeyPress(Keys.Back, _screen.ControllingPlayer, out player))
            {
                if (_editingNameTemp.Length >= 1)
                {
                    _editingNameTemp = _editingNameTemp.Substring(0, _editingNameTemp.Length - 1);   
                }
            }
            var playerIndex = (int)_screen.ControllingPlayer.Value;
            var keyState = input.CurrentKeyboardStates[playerIndex];
            var lastState = input.LastKeyboardStates[playerIndex];
            if (lastState == keyState)
            {
                return;
            }
            if (_lastWrittenKey != Keys.None)
            {
                _lastWrittenKey = Keys.None;
                return;
            }
            var keys = keyState.GetPressedKeys();
            var uppercase = keys.Where(k => k == Keys.LeftShift || k == Keys.RightShift).FirstOrDefault() != Keys.None;
            foreach (var key in keys)
            {

                if (
                    (key >= Keys.A && key <= Keys.Z) ||
                    (key >= Keys.D0 && key <= Keys.D9) ||
                    (key == Keys.Space) ||
                    (key == Keys.OemComma) ||
                    (key == Keys.OemPeriod) ||
                    (key == Keys.OemMinus)
                    )
                {
                    var character = TranslateKeyToString(key);
                    if (!uppercase)
                    {
                        character = character.ToLower();
                    }
                    _editingNameTemp += character;
                    _lastWrittenKey = key;
                }
                else
                {
                    _lastWrittenKey = Keys.None;
                }
            }
            
        }

        private void HandleEditTypeInput(InputState input)
        {
            PlayerIndex player;

            if (input.IsNewKeyPress(Keys.Enter, _screen.ControllingPlayer, out player))
            {
                _playerDefinitions[SelectedId].IsAi = _editingTypeTemp;
                _playerDefinitions[SelectedId].Active = _editingTypeTemp;
                SaveSettings();
                _editingType = false;
            }
            if (input.IsNewKeyPress(Keys.Escape, _screen.ControllingPlayer, out player))
            {
                _editingType = false;
            }

            if (input.IsNewKeyPress(Keys.Up, _screen.ControllingPlayer, out player) || input.IsNewKeyPress(Keys.Down, _screen.ControllingPlayer, out player))
            {
                _editingTypeTemp = !_editingTypeTemp;
            }
        }

        private void HandleEditKeysInput(InputState input)
        {
            PlayerIndex player;

            if (input.IsNewKeyPress(Keys.Enter, _screen.ControllingPlayer, out player) || _editKeysRound >= 2)
            {
                _playerDefinitions[SelectedId].Left = _editingKeysTemp.Item1;
                _playerDefinitions[SelectedId].Right = _editingKeysTemp.Item2;
                SaveSettings();
                _editKeysRound = 0;
                _editingKeys = false;
            }
            if (input.IsNewKeyPress(Keys.Escape, _screen.ControllingPlayer, out player))
            {
                _editingKeys = false;
                _editKeysRound = 0;
            }

            var playerIndex = (int)_screen.ControllingPlayer.Value;
            var keyState = input.CurrentKeyboardStates[playerIndex];
           
            var keys = keyState.GetPressedKeys();
            Keys currentKey = keys.Where(key => !_forbiddenControls.Contains(key)).FirstOrDefault();
            if (currentKey == Keys.None || currentKey == null) return;
            if (_editKeysRound == 0)
            {
                _editingKeysTemp = new Tuple<Keys, Keys>(currentKey, _editingKeysTemp.Item2);
                _editKeysRound++;
            }
            else
            {
                if (currentKey == _editingKeysTemp.Item1)
                {
                    return;
                }
                _editingKeysTemp = new Tuple<Keys, Keys>(_editingKeysTemp.Item1, currentKey);
                _editKeysRound++;
            }
        }

        private void DrawHelp(Rectangle container)
        {
            spriteBatch.DrawString(CommonResources.fontSmall, "[Up, Down] Select player", new Vector2(container.X + 10, container.Y + 10), CommonResources.Borders);
            spriteBatch.DrawString(CommonResources.fontSmall, "[Left, Right] Select property", new Vector2(container.X + 10, container.Y + 35), CommonResources.Borders);
            spriteBatch.DrawString(CommonResources.fontSmall, "[Enter] Edit", new Vector2(container.X + 10, container.Y + 60), CommonResources.Borders);
            //spriteBatch.DrawString(CommonResources.fontSmall, "[K] Change key bindings", new Vector2(container.X + 10, container.Y + 60), CommonResources.Borders);
            spriteBatch.DrawString(CommonResources.fontSmall, "[A] De/activate AI", new Vector2(container.X + 10, container.Y + 85), CommonResources.Borders);
            spriteBatch.DrawString(CommonResources.fontSmall, "[P] Add new player", new Vector2(container.X + 10, container.Y + 110), CommonResources.Borders);
            spriteBatch.DrawString(CommonResources.fontSmall, "[Del] Delete player", new Vector2(container.X + 10, container.Y + 135), CommonResources.Borders);
            spriteBatch.DrawString(CommonResources.fontSmall, "[Esc] Back", new Vector2(container.X + 10, container.Y + 160), CommonResources.Borders);
        }

        private Rectangle GetHelpRectangle()
        {
             return new Rectangle(
                ContentArea.X +1,
                ContentArea.Y - 249,
                ContentArea.Width -2,
                188
            ); 
        }

        private Rectangle GetHelpRectangleBorder()
        {
            return new Rectangle(
               ContentArea.X,
               ContentArea.Y - 250,
               ContentArea.Width ,
               190
           );
        }

        private Rectangle GetBorderRectangle()
        {
            return new Rectangle(
                ContentArea.X,
                ContentArea.Y,
                ContentArea.Width,
                (int)(_playerDefinitions.Count * 42f)
                );
        }

        private Rectangle GetContentRectangle()
        {
            return new Rectangle(
                ContentArea.X +1,
                ContentArea.Y +1,
                ContentArea.Width -2,
                (int)(_playerDefinitions.Count * 42f) -2
                );
        }

        public void SaveSettings()
        {
            //RegistrySettings.WriteString("Definition", JsonConvert.SerializeObject(_playerDefinitions), RegistrySection.Players);
            RegistrySettings.Write("Players", _playerDefinitions);
        }

        private Color CreateColorTransition(Color color)
        {
            var colors = color.ToVector4();
            colors.X = colors.Y = colors.Z = MathHelper.Clamp((float)Math.Sin(_colorTransition), 0.3f, 0.49f);
            _colorTransition = _colorTransition + 0.3f;
            return new Color(colors);
        }

        private string TranslateKeyToString(Keys key)
        {
            switch (key)
            {
                case Keys.Space:
                    return " ";
                case Keys.OemComma:
                    return ",";
                case Keys.OemMinus:
                    return "-";
                case Keys.OemPeriod:
                    return ".";
                default:
                    return key.ToString();
            }
        }
    }
}
