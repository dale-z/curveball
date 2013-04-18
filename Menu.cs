using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tutorial16___Multiple_Viewport___PhoneLib;

namespace Curveball
{
    // The viewport and ground marker node comes from the container.
    // This class only contains menu specific scene elements.
    public class Menu
    {
        public Menu(Tutorial16_Phone container)
        {
            _container = container;
        }

        public void GotoLevel()
        {
            // What to do here:
            // Halt the container update.
            // Unmount itself. (Call this.Unmount().)
            // Create a level with the information gathered.
            // Mount the level. (Call level.Mount().)
            // Set the state of the container.
            // Resume container update.
        }

        public void Mount()
        {
            // Attach its elements to the scene graph in the container.

            throw new NotImplementedException();
        }

        public void Unmount()
        {
            // Detach its elements from the scene graph in the container.

            throw new NotImplementedException();
        }

        private Tutorial16_Phone _container;

        // TODO
        // Move the menu specific part of the scene graph from the
        // container to this class.
    }
}
