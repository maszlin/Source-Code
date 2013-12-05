using System;
using System.Text;
using Microsoft.VisualBasic;

namespace NEPS.OSP.COPPER.COPY.DP
{
    public class InputBox
    {
        /* Class calling VB's InputBox function
         * Author: m.zam
         * Developed: march-2012
         * Modified : 28-09-2012 @ add owner form - set topmost to false
         */

        /// <summary>
        /// Displays a prompt in a dialog box, waits for the user to input text or click a button, and then returns a string containing the contents of the text box.
        /// </summary>
        /// <param name="Prompt">String expression displayed as the message in the dialog box. The maximum length of Prompt is approximately 1024 characters, depending on the width of the characters used. If Prompt consists of more than one line, you can separate the lines using a carriage return character (\r), a linefeed character (\n), or a carriage return–linefeed character combination (\r\n) between each line.</param>
        /// <param name="Title">String expression displayed in the title bar of the dialog box.</param>
        /// <param name="DefaultResponse">String expression displayed in the text box as the default response if no other input is provided.</param>
        /// <param name="xPos">Numeric expression that specifies the left edge of the dialog box. If xPos is -1, the dialog box is centered horizontally.</param>
        /// <param name="yPos">Numeric expression that specifies the top edge of the dialog box. If yPos is -1, the dialog box is centered vertically.</param>
        /// <returns>A string containing the user's response.</returns>
        public static string Show(frmCopyDP owner, string Prompt, string Title, string DefaultResponse, int xPos, int yPos)
        {
            bool topmost = owner.TopMost;
            try
            {
                owner.TopMost = false;
                string reply = Interaction.InputBox(Prompt, Title, DefaultResponse, xPos, yPos);
                return reply;
            }
            catch
            {
                return "";
            }
            finally
            {
                owner.TopMost = topmost;
            }
        }
    }
}
