﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QControls.Utils.Validation
{
    public interface IValidateModel
    {
        void Validate(ModelStateDictionary modelState);
    }
}
