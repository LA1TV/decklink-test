using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeckLinkAPI;

namespace DeckLinkTest
{
    internal class Device
    {
        public IDeckLink Card { get; set; }
        public String ModelName { get; set; }
        public String DisplayName { get; set; }

        public IDeckLinkOutput DeckLinkOutput
        {
            get { return (IDeckLinkOutput)Card; }
        }

        public IDeckLinkDisplayMode GetDisplayMode(string modeName)
        {
            IDeckLinkDisplayModeIterator displayModeIterator;

            // Get the IDeckLinkOutput interface
            DeckLinkOutput.GetDisplayModeIterator(out displayModeIterator);

            while (true)
            {
                IDeckLinkDisplayMode deckLinkDisplayMode;
                string name, rawName;

                displayModeIterator.Next(out deckLinkDisplayMode);

                if (deckLinkDisplayMode == null)
                    break;

                deckLinkDisplayMode.GetName(out rawName);

                name = rawName.Replace(" ", string.Empty);

                if (name.Equals(modeName))
                    return deckLinkDisplayMode;
            }

            throw new Exception("Invalid Mode");
        }
    }
}
