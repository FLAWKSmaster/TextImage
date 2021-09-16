using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TextImageApp.Models
{
    public class TextModel
    {
        public string TextInput { get; set; }
        public string TextOutput { get; set; }

        [DisplayName("TextOnImage")]
        public string TextOnImage { get; set; }
    }
}