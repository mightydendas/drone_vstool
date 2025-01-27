using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroCo.Editor {
    internal interface IEditorContainerPage {
        void OnEnable();
        void OnGUI();
    }
}
