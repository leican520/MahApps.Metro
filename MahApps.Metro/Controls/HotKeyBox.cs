﻿using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using MahApps.Metro.Native;

namespace MahApps.Metro.Controls {
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    public class HotKeyBox : Control {
        private const string PART_TextBox = "PART_TextBox";

        public static readonly DependencyProperty HotKeyProperty = DependencyProperty.Register(
            "HotKey", typeof(HotKey), typeof(HotKeyBox),
            new FrameworkPropertyMetadata(default(HotKey), OnHotKeyChanged) { BindsTwoWayByDefault = true });

        public HotKey HotKey
        {
            get { return (HotKey) GetValue(HotKeyProperty); }
            set { SetValue(HotKeyProperty, value); }
        }

        private static void OnHotKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (HotKeyBox)d;
            ctrl.UpdateText();
        }

        public static readonly DependencyProperty AreModifierKeysRequiredProperty = DependencyProperty.Register(
            "AreModifierKeysRequired", typeof(bool), typeof(HotKeyBox), new PropertyMetadata(default(bool)));

        public bool AreModifierKeysRequired
        {
            get { return (bool) GetValue(AreModifierKeysRequiredProperty); }
            set { SetValue(AreModifierKeysRequiredProperty, value); }
        }

        private TextBox _textBox;

        static HotKeyBox() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HotKeyBox), new FrameworkPropertyMetadata(typeof(HotKeyBox)));
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _textBox = Template.FindName(PART_TextBox, this) as TextBox;
            if (_textBox != null) {
                _textBox.PreviewKeyDown += TextBoxOnPreviewKeyDown2;
                _textBox.GotFocus += TextBoxOnGotFocus;
                _textBox.LostFocus += TextBoxOnLostFocus;
                UpdateText();
            }
        }

        private void TextBoxOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcherOnThreadPreprocessMessage;
        }

        private void ComponentDispatcherOnThreadPreprocessMessage(ref MSG msg, ref bool handled) {
            if (msg.message == Constants.WM_HOTKEY)
            {
                // swallow all hotkeys, so our control can catch the key strokes
                handled = true;
            }
        }

        private void TextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            ComponentDispatcher.ThreadPreprocessMessage -= ComponentDispatcherOnThreadPreprocessMessage;
        }

        private void TextBoxOnPreviewKeyDown2(object sender, KeyEventArgs e) {
            var key = (e.Key == Key.System ? e.SystemKey : e.Key);
            switch (key) {
                case Key.Tab:
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.RWin:
                case Key.LWin:
                    return;
            }

            e.Handled = true;

            var currentModifierKeys = GetCurrentModifierKeys();
            if (currentModifierKeys == ModifierKeys.None && key == Key.Back)
            {
                HotKey = null;
            }
            else if (currentModifierKeys != ModifierKeys.None || !AreModifierKeysRequired)
            {
                HotKey = new HotKey(key, currentModifierKeys);
            }

            UpdateText();
        }

        private static ModifierKeys GetCurrentModifierKeys() {
            var modifier = ModifierKeys.None;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                modifier |= ModifierKeys.Control;
            }
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) {
                modifier |= ModifierKeys.Alt;
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                modifier |= ModifierKeys.Shift;
            }
            if (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin)) {
                modifier |= ModifierKeys.Windows;
            }
            return modifier;
        }

        private void UpdateText() {
            if (_textBox == null) {
                return;
            }

            var hotkey = HotKey;
            if (hotkey == null || hotkey.Key == Key.None) {
                _textBox.Text = string.Empty;
                return;
            }

            _textBox.Text = hotkey.ToString();
        }
    }

    public class HotKey : IEquatable<HotKey>
    {
        private readonly Key _key;
        private readonly ModifierKeys _modifierKeys;

        public HotKey(Key key, ModifierKeys modifierKeys)
        {
            _key = key;
            _modifierKeys = modifierKeys;
        }

        public Key Key
        {
            get { return _key; }
        }

        public ModifierKeys ModifierKeys
        {
            get { return _modifierKeys; }
        }

        public override bool Equals(object obj)
        {
            return obj is HotKey && Equals((HotKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) _key*397) ^ (int) _modifierKeys;
            }
        }

        public bool Equals(HotKey other)
        {
            return _key == other._key && _modifierKeys == other._modifierKeys;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            if ((_modifierKeys & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                sb.Append(GetLocalizedKeyStringUnsafe(Constants.VK_MENU));
                sb.Append("+");
            }
            if ((_modifierKeys & ModifierKeys.Control) == ModifierKeys.Control)
            {
                sb.Append(GetLocalizedKeyStringUnsafe(Constants.VK_CONTROL));
                sb.Append("+");
            }
            if ((_modifierKeys & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                sb.Append(GetLocalizedKeyStringUnsafe(Constants.VK_SHIFT));
                sb.Append("+");
            }
            if ((_modifierKeys & ModifierKeys.Windows) == ModifierKeys.Windows)
            {
                sb.Append("WINDOWS+");
            }
            sb.Append(GetLocalizedKeyStringUnsafe(KeyInterop.VirtualKeyFromKey(_key)).ToUpper());
            return sb.ToString();
        }

        private static string GetLocalizedKeyStringUnsafe(int key)
        {
            // strip any modifier keys
            long keyCode = key & 0xffff;

            var sb = new StringBuilder(256);

            long scanCode = UnsafeNativeMethods.MapVirtualKey((uint)keyCode, Constants.MAPVK_VK_TO_VSC);

            // shift the scancode to the high word
            scanCode = (scanCode << 16);
            if (keyCode == 45 ||
                keyCode == 46 ||
                keyCode == 144 ||
                (33 <= keyCode && keyCode <= 40))
            {
                // add the extended key flag
                scanCode |= 0x1000000;
            }

            UnsafeNativeMethods.GetKeyNameText((int)scanCode, sb, 256);
            return sb.ToString();
        }

    }
}