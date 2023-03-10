using System;
using System.Drawing;
using System.Drawing.Printing;
using C1.C1Preview;
using C1.Win.C1FlexGrid;

namespace Payroll.CFC
{
	/// <summary>
	/// Object to manage custom page headers and footers when printing and previewing.
	/// </summary>
    public class C1OwnerDrawPrint
    {
        private const double DefaultHeightInch = 1;

        private double _height = DefaultHeightInch;
        private RenderArea _rootObject = null;
        private C1PrintDocument _doc = null;

        internal C1OwnerDrawPrint(C1.C1Preview.C1PrintDocument doc)
        {
            _doc = doc;
        }

        /// <summary>
        /// Set the height of the printing area (in inches)
        /// </summary>
        /// <param name="value"></param>
        internal void SetHeightInch(double value)
        {
            if (value >= 0)
                _height = value;
            else
                _height = 0;
        }

        /// <summary>
        /// Gets the height of the header/footer area in inches.
        /// </summary>
        public double HeightInch
        {
            get { return _height; }
        }

        internal void StartDrawing()
        {
            _rootObject = new RenderArea(_doc);
            _rootObject.Height = new Unit(_height, UnitTypeEnum.Inch);
        }
        
        internal void EndDrawing()
        {
        }

        internal RenderObject GetRootObject()
        {
            return _rootObject;
        }

        /// <summary>
        /// Gets the RenderArea object representing the entire area of the
        /// owner-drawn page header or footer.
        /// </summary>
        public RenderArea RenderArea
        {
            get { return _rootObject; }
        }

        /// <summary>
        /// Adds text to the owner-drawn page header or footer.
        /// </summary>
        /// <param name="x">The X coordinate of the text.</param>
        /// <param name="y">The Y coordinate of the text.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="width">The width of the text area.</param>
        /// <param name="height">The height of the text area.</param>
        /// <param name="font">The text font.</param>
        /// <param name="textColor">The text color.</param>
        /// <param name="align">The text alignment.</param>
        /// <remarks>
        /// The <paramref name="text"/> parameter may contain the following placeholders:
        /// <list type="bullet">
        /// <item><c>\p</c> - current page number (equivalent to C1PrintDocument tag <c>[PagenNo]</c></item>
        /// <item><c>\P</c> - total page count (equivalent to C1PrintDocument tag <c>[PageCount]</c></item>
        /// <item><c>\s</c> - horizontal page number (equivalent to C1PrintDocument tag <c>[PageX]</c></item>
        /// <item><c>\S</c> - total horizontal page count (equivalent to C1PrintDocument tag <c>[PageXCount]</c></item>
        /// <item><c>\g</c> - vertical page number (equivalent to C1PrintDocument tag <c>[PageY]</c></item>
        /// <item><c>\G</c> - total vertical page count (equivalent to C1PrintDocument tag <c>[PageYCount]</c></item>
        /// </list>
        /// </remarks>
        public void RenderDirectText(Unit x, Unit y, string text, Unit width, Unit height,
            Font font, Color textColor, TextAlignEnum align)
        {
            RenderText dText = new RenderText(_doc);
            switch (align)
            {
                case TextAlignEnum.CenterBottom:
                    dText.Style.TextAlignHorz = AlignHorzEnum.Center;
                    dText.Style.TextAlignVert = AlignVertEnum.Bottom;
                    break;
                case TextAlignEnum.CenterCenter:
                    dText.Style.TextAlignHorz = AlignHorzEnum.Center;
                    dText.Style.TextAlignVert = AlignVertEnum.Center;
                    break;
                case TextAlignEnum.CenterTop:
                    dText.Style.TextAlignHorz = AlignHorzEnum.Center;
                    dText.Style.TextAlignVert = AlignVertEnum.Top;
                    break;
                case TextAlignEnum.GeneralBottom:
                    dText.Style.TextAlignHorz = AlignHorzEnum.Left;
                    dText.Style.TextAlignVert = AlignVertEnum.Bottom;
                    break;
                case TextAlignEnum.GeneralCenter:
                    dText.Style.TextAlignHorz = AlignHorzEnum.Left;
                    dText.Style.TextAlignVert = AlignVertEnum.Center;
                    break;
                case TextAlignEnum.GeneralTop:
                    dText.Style.TextAlignHorz = AlignHorzEnum.Left;
                    dText.Style.TextAlignVert = AlignVertEnum.Top;
                    break;
                case TextAlignEnum.LeftBottom:
                    dText.Style.TextAlignHorz = AlignHorzEnum.Left;
                    dText.Style.TextAlignVert = AlignVertEnum.Bottom;
                    break;
                case TextAlignEnum.LeftCenter:
                    dText.Style.TextAlignHorz = AlignHorzEnum.Left;
                    dText.Style.TextAlignVert = AlignVertEnum.Center;
                    break;
                case TextAlignEnum.LeftTop:
                    dText.Style.TextAlignHorz = AlignHorzEnum.Left;
                    dText.Style.TextAlignVert = AlignVertEnum.Top;
                    break;
                case TextAlignEnum.RightBottom:
                    dText.Style.TextAlignHorz = AlignHorzEnum.Right;
                    dText.Style.TextAlignVert = AlignVertEnum.Bottom;
                    break;
                case TextAlignEnum.RightCenter:
                    dText.Style.TextAlignHorz = AlignHorzEnum.Right;
                    dText.Style.TextAlignVert = AlignVertEnum.Center;
                    break;
                case TextAlignEnum.RightTop:
                    dText.Style.TextAlignHorz = AlignHorzEnum.Right;
                    dText.Style.TextAlignVert = AlignVertEnum.Top;
                    break;
            }
            dText.Text = C1FlexGridPrintable.ReplacePageText(text);
            dText.Style.Font = font;
            dText.Style.TextColor = textColor;
            dText.Width = width;
            dText.Height = height;
            dText.X = x;
            dText.Y = y;
            _rootObject.Children.Add(dText);
        }

        /// <summary>
        /// Adds text to the owner-drawn page header or footer.
        /// </summary>
        /// <param name="x">The X coordinate of the text.</param>
        /// <param name="y">The Y coordinate of the text.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="width">The width of the text area.</param>
        /// <param name="font">The text font.</param>
        /// <param name="textColor">The text color.</param>
        /// <param name="align">The text alignment.</param>
        /// <remarks>
        /// The <paramref name="text"/> parameter may contain the following placeholders:
        /// <list type="bullet">
        /// <item><c>\p</c> - current page number (equivalent to C1PrintDocument tag <c>[PagenNo]</c></item>
        /// <item><c>\P</c> - total page count (equivalent to C1PrintDocument tag <c>[PageCount]</c></item>
        /// <item><c>\s</c> - horizontal page number (equivalent to C1PrintDocument tag <c>[PageX]</c></item>
        /// <item><c>\S</c> - total horizontal page count (equivalent to C1PrintDocument tag <c>[PageXCount]</c></item>
        /// <item><c>\g</c> - vertical page number (equivalent to C1PrintDocument tag <c>[PageY]</c></item>
        /// <item><c>\G</c> - total vertical page count (equivalent to C1PrintDocument tag <c>[PageYCount]</c></item>
        /// </list>
        /// </remarks>
        public void RenderDirectText(Unit x, Unit y, string text, Unit width,
            Font font, Color textColor, TextAlignEnum align)
        {
            RenderDirectText(x, y, text, width, Unit.Auto, font, textColor, align);
        }

        /// <summary>
        /// Adds text to the owner-drawn page header or footer.
        /// </summary>
        /// <param name="x">The X coordinate of the text.</param>
        /// <param name="y">The Y coordinate of the text.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="width">The width of the text area.</param>
        /// <param name="font">The text font.</param>
        /// <param name="textColor">The text color.</param>
        /// <param name="align">The text alignment.</param>
        /// <remarks>
        /// The <paramref name="text"/> parameter may contain the following placeholders:
        /// <list type="bullet">
        /// <item><c>\p</c> - current page number (equivalent to C1PrintDocument tag <c>[PagenNo]</c></item>
        /// <item><c>\P</c> - total page count (equivalent to C1PrintDocument tag <c>[PageCount]</c></item>
        /// <item><c>\s</c> - horizontal page number (equivalent to C1PrintDocument tag <c>[PageX]</c></item>
        /// <item><c>\S</c> - total horizontal page count (equivalent to C1PrintDocument tag <c>[PageXCount]</c></item>
        /// <item><c>\g</c> - vertical page number (equivalent to C1PrintDocument tag <c>[PageY]</c></item>
        /// <item><c>\G</c> - total vertical page count (equivalent to C1PrintDocument tag <c>[PageYCount]</c></item>
        /// </list>
        /// </remarks>
        public void RenderDirectText(object x, object y, string text, object width,
            Font font, Color textColor, TextAlignEnum align)
        {
            RenderDirectText(new Unit(x.ToString()), new Unit(y.ToString()), text,
                new Unit(width.ToString()), font, textColor, align);
        }

        /// <summary>
        /// Adds image to the owner-drawn page header or footer.
        /// </summary>
        /// <param name="x">The X coordinate of the image.</param>
        /// <param name="y">The Y coordinate of the image.</param>
        /// <param name="image">The image to render.</param>
        /// <param name="width">The image width.</param>
        /// <param name="height">The image height.</param>
        /// <param name="align">Image alignment options.</param>
        public void RenderDirectImage(Unit x, Unit y, Image image,
            Unit width, Unit height, ImageAlignEnum align)
        {
            RenderImage dImage = new RenderImage(_doc);
            dImage.Image = image;
            dImage.Width = width;
            dImage.Height = height;
            ImageAlign ia = ImageAlign.Default;
            switch (align)
            {
                case ImageAlignEnum.CenterCenter:
                    ia.AlignHorz = ImageAlignHorzEnum.Center;
                    ia.AlignVert = ImageAlignVertEnum.Center;
                    break;
                case ImageAlignEnum.CenterTop:
                    ia.AlignHorz = ImageAlignHorzEnum.Center;
                    ia.AlignVert = ImageAlignVertEnum.Top;
                    break;
                case ImageAlignEnum.CenterBottom:
                    ia.AlignHorz = ImageAlignHorzEnum.Center;
                    ia.AlignVert = ImageAlignVertEnum.Bottom;
                    break;
                case ImageAlignEnum.LeftBottom:
                    ia.AlignHorz = ImageAlignHorzEnum.Left;
                    ia.AlignVert = ImageAlignVertEnum.Bottom;
                    break;
                case ImageAlignEnum.LeftCenter:
                    ia.AlignHorz = ImageAlignHorzEnum.Left;
                    ia.AlignVert = ImageAlignVertEnum.Center;
                    break;
                case ImageAlignEnum.LeftTop:
                    ia.AlignHorz = ImageAlignHorzEnum.Left;
                    ia.AlignVert = ImageAlignVertEnum.Top;
                    break;
                case ImageAlignEnum.RightBottom:
                    ia.AlignHorz = ImageAlignHorzEnum.Right;
                    ia.AlignVert = ImageAlignVertEnum.Bottom;
                    break;
                case ImageAlignEnum.RightCenter:
                    ia.AlignHorz = ImageAlignHorzEnum.Right;
                    ia.AlignVert = ImageAlignVertEnum.Center;
                    break;
                case ImageAlignEnum.RightTop:
                    ia.AlignHorz = ImageAlignHorzEnum.Right;
                    ia.AlignVert = ImageAlignVertEnum.Top;
                    break;
                case ImageAlignEnum.Scale:
                    ia.KeepAspectRatio = true;
                    break;
                case ImageAlignEnum.Stretch:
                    ia.KeepAspectRatio = false;
                    ia.StretchHorz = true;
                    ia.StretchVert = true;
                    break;
                case ImageAlignEnum.Tile:
                    ia.TileHorz = true;
                    ia.TileVert = true;
                    break;
            }
            dImage.Style.ImageAlign = ia;
            _rootObject.Children.Add(dImage);
        }

        /// <summary>
        /// Adds image to the owner-drawn page header or footer.
        /// </summary>
        /// <param name="x">X-coordinate.</param>
        /// <param name="y">Y-coordinate.</param>
        /// <param name="image">Image to render.</param>
        /// <param name="width">Width to render.</param>
        /// <param name="height">Height to render.</param>
        /// <param name="align">Alignment options.</param>
        public void RenderDirectImage(object x, object y, Image image,
            object width, object height, ImageAlignEnum align)
        {
            RenderDirectImage(new Unit(x.ToString()), new Unit(y.ToString()), image,
                new Unit(width.ToString()), new Unit(height.ToString()), align);
        }
    }
}