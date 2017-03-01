/*
    Space Reserver - Class for Reserving Space on Desktop
    Copyright (C) 2017  Piotr 'MiXer' Mikstacki

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>
 */
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LightDE
{
    /// <summary>
    /// Description of SpaceReserver.
    /// </summary>
    public class SpaceReserver
    {
        public enum SPI
        {
            SPI_SETWORKAREA = 47,
            SPI_GETWORKAREA
        }

        public struct RECT
        {
            public int left;

            public int top;

            public int right;

            public int bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref SpaceReserver.RECT pvParam, uint fWinIni);
        /// <summary>
        /// Set's offset for Shell's UI Elements
        /// </summary>
        /// <param name="offsetLeft"></param>
        /// <param name="offsetTop"></param>
        /// <param name="offsetRight"></param>
        /// <param name="offsetBottom"></param>
        public static void MakeNewDesktopArea(int offsetLeft, int offsetTop, int offsetRight, int offsetBottom)
        {
            SpaceReserver.RECT rECT;
            rECT.left = SystemInformation.VirtualScreen.Left + offsetLeft;
            rECT.top = SystemInformation.VirtualScreen.Top + offsetTop;
            rECT.right = SystemInformation.VirtualScreen.Right - offsetRight;
            rECT.bottom = SystemInformation.VirtualScreen.Bottom - offsetBottom;
            SpaceReserver.SystemParametersInfo(47u, 0u, ref rECT, 0u);
        }
    }
}
