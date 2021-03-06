﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ninject;
using Ninject.Modules;

namespace Handlebars
{
    public sealed class NodeModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IHandlebarsEngine>().To<NodeEngine>().InSingletonScope();
        }
    }
}
