﻿using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Security.Controllers;
using Xpand.ExpressApp.Security.Registration;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;
using Xpand.Persistent.Base.Validation;
using ChooseDatabaseAtLogonController = Xpand.ExpressApp.Security.Controllers.ChooseDatabaseAtLogonController;

namespace Xpand.ExpressApp.Security {
    public abstract class XpandSecurityModuleBase:XpandModuleBase {
        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            if (Application != null)
                Application.CreateCustomLogonWindowControllers += application_CreateCustomLogonWindowControllers;
        }

        private void application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            var controllers= e.Controllers.ToDictionary(controller => controller.GetType());
            e.Controllers.Clear();
            if (((IModelOptionsRegistration) Application.Model.Options).Registration.Enabled)
                AddRegistrationControllers((XafApplication)sender, controllers);
            if (SecuritySystem.LogonParameters is IDBServerParameter){
                AddControllers(controllers,Application.CreateAppearanceControllers());
            }
            if (((IModelOptionsChooseDatabaseAtLogon) Application.Model.Options).ChooseDatabaseAtLogon){
                AddControllers(controllers, Application.CreateValidationControllers().Concat(new[] { Application.CreateController<ChooseDatabaseAtLogonController>() }));
            }
            e.Controllers.AddRange(controllers.Select(pair => pair.Value));
        }

        protected void AddControllers(Dictionary<Type, Controller> controllers, IEnumerable<Controller> controllersToAdd){
            foreach (var controller in controllersToAdd) {
                if (!controllers.ContainsKey(controller.GetType())){
                    controllers.Add(controller.GetType(),controller);
                }
            }
        }

        protected virtual void AddRegistrationControllers(XafApplication application, Dictionary<Type, Controller> controllers) {
            var registrationControllers = CreateRegistrationControllers(application).ToArray();
            var appearenceControllers = application.CreateAppearanceControllers();
            var validationControllers = application.CreateValidationControllers();
            var allControllers = registrationControllers.Concat(appearenceControllers).Concat(validationControllers);
            AddControllers(controllers, allControllers);
        }

        public static IEnumerable<Controller> CreateRegistrationControllers(XafApplication app) {
            var typeInfo = app.TypesInfo.FindTypeInfo(typeof(IPasswordScoreController)).Implementors.FirstOrDefault();
            if (typeInfo != null)
                yield return app.CreateController(typeInfo.Type);

            var manageUsedOnLogonTypeInfo = app.TypesInfo.FindTypeInfo(typeof(ManageUsersOnLogonController));
            var manageUsedOnLogonType = manageUsedOnLogonTypeInfo.Descendants.Select(info => info.Type).FirstOrDefault() ??manageUsedOnLogonTypeInfo.Type;
            yield return app.CreateController(manageUsedOnLogonType);
        }
    }
}
