using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SimpleRemote.Controls.Dragablz
{
    public class HorizontalOrganiser : StackOrganiser
    {
        public HorizontalOrganiser() : base(Orientation.Horizontal)
        { }

        public HorizontalOrganiser(double itemOffset) : base(Orientation.Horizontal, itemOffset)
        { }
    }    
}