﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.Animation;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Widget;
using Skor.Controls.Abstractions;
using Skor.Controls.Droid.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
[assembly: ExportRenderer(typeof(global::Skor.Controls.GradientButton), typeof(global::Skor.Controls.Droid.GradientButtonRenderer))]
namespace Skor.Controls.Droid
{
    public class GradientButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<global::Skor.Controls.GradientButton, FrameLayout>
    {
        private const int DEFAULT_HEIGHT_BUTTON = 56;
        private Android.Support.V7.Widget.AppCompatButton nButton;
        private global::Skor.Controls.GradientButton button;
        private FrameLayout frame;
        public GradientButtonRenderer(Context context) : base(context)
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<global::Skor.Controls.GradientButton> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null) return;

            this.button = e.NewElement as global::Skor.Controls.GradientButton;
            this.button.HeightRequest = this.button.HeightRequest >= DEFAULT_HEIGHT_BUTTON ? this.button.HeightRequest : DEFAULT_HEIGHT_BUTTON;
            InitControls();
            RenderText();

            nButton.Background = CreateBackgroundForButton();

            nButton.Click += (s, ev) =>
            {
                button.SendClicked();
            };
            nButton.LongClick += (s, ev) =>
            {
                button.SendLongClick();
            };
            InitStyleButton();
            frame.AddView(nButton);
            SetNativeControl(this.frame);
        }
        void InitControls()
        {
            //Layout
            frame = new FrameLayout(Context);
            frame.LayoutParameters = new FrameLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
            nButton = new Android.Support.V7.Widget.AppCompatButton(Context);
            //Button
            var nBtnLayout = new FrameLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
            nBtnLayout.SetMargins(8, 8, 8, 24);
            nButton.SetPadding(0, 0, 0, 0);
            nButton.LayoutParameters = nBtnLayout;
        }
        void RenderText()
        {
            nButton.Text = button.Text;
            nButton.SetTextColor(button.TextColor.ToAndroid());
            nButton.Typeface = button.Font.ToTypeface();
        }
        private void InitStyleButton()
        {
            nButton.AddRipple(button.RippleColor.ToAndroid());
            nButton.Enabled = button.IsEnabled;
            if (!button.HasShadow)
            {
                nButton.StateListAnimator = null;
            }
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(button.IsEnabled))
            {
                nButton.Enabled = button.IsEnabled;
            }
            if (e.PropertyName == nameof(button.StartColor) || e.PropertyName == nameof(button.EndColor)
                || e.PropertyName == nameof(button.CenterColor))
            {
                if(nButton!=null)
                    nButton.Background = CreateBackgroundForButton();
            }
        }
        Drawable CreateBackgroundForButton(bool renderImage = true)
        {
            List<Drawable> drawables = new List<Drawable>();
            List<Drawable> drawablesDisabled = new List<Drawable>();
            drawables.Add(BackgroundExtension.CreateBackgroundGradient(button.StartColor.ToAndroid(),
                button.EndColor.ToAndroid(),
                button.CenterColor.ToAndroid(),
                button.CornerRadius,
                button.Angle.ToAndroid()));
            drawablesDisabled.Add(BackgroundExtension.CreateBackgroundGradient(button.StartColor.ToAndroid(),
                button.EndColor.ToAndroid(),
                button.CenterColor.ToAndroid(),
                button.CornerRadius,
                button.Angle.ToAndroid()));
            if (button.Image != null && !string.IsNullOrEmpty(button.Image.File) && renderImage)
            {
                var bitmapDrawable = BackgroundExtension.CreateBackgroundBitmap(button.Image.File, (int)button.HeightRequest,
                    (int)button.WidthRequest, button.CornerRadius);
                if (bitmapDrawable != null)
                {
                    drawables.Add(bitmapDrawable);
                    drawablesDisabled.Add(bitmapDrawable);
                }
            }
            Drawable layer = new LayerDrawable(drawables.ToArray());
            Drawable layerDisabled = new LayerDrawable(drawablesDisabled.ToArray());
            layerDisabled.Mutate().Alpha = 80;
            var statesListDrawable = new StateListDrawable();
            statesListDrawable.AddState(new int[] { -Android.Resource.Attribute.StateEnabled }, layerDisabled);
            statesListDrawable.AddState(new int[] { Android.Resource.Attribute.StatePressed }, layer);
            statesListDrawable.AddState(new int[] { Android.Resource.Attribute.StateEnabled }, layer);
            return statesListDrawable;
        }
    }
}