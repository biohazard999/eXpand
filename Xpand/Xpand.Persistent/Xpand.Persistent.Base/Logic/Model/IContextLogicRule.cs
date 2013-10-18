﻿using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Logic.Model {
    [RuleCriteria("IContextLogicRule", DefaultContexts.Save, "(Not IsNullOrEmpty(ExecutionContextGroup)) OR (Not IsNullOrEmpty(ActionExecutionContextGroup))", SkipNullOrEmptyValues = false, CustomMessageTemplate = "At least one of ExecutionContextGroup, ActionExecutionContextGroup should not be null")]
    [ModelAbstractClass]
    public interface IContextLogicRule : ILogicRule {
        [DataSourceProperty("FrameTemplateContexts")]
        [Category("Logic.Data")]
        [ModelBrowsable(typeof(FrameTemplateContextGroupVisibilityCalculator))]
        string FrameTemplateContextGroup { get; set; }

        [Category("Logic.Data")]
        [DataSourceProperty("ExecutionContexts")]
        string ExecutionContextGroup { get; set; }

        [Category("Logic.Data")]
        [DataSourceProperty("ActionExecutionContexts")]
        [ModelBrowsable(typeof(ActionExecutionContextGroupVisibilityCalculator))]
        string ActionExecutionContextGroup { get; set; }

        [Category("Logic.Data")]
        [DataSourceProperty("ViewContexts")]
        [ModelBrowsable(typeof(ViewContextGroupVisibilityCalculator))]
        string ViewContextGroup { get; set; }
    }

    public class FrameTemplateContextGroupVisibilityCalculator:IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return !Equals(((IModelLogicRule) node).ModelLogicWrapper.FrameTemplateContextsGroup,Enumerable.Empty<IModelFrameTemplateContexts>());
        }
    }

    public class ViewContextGroupVisibilityCalculator:IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return !Equals(((IModelLogicRule) node).ModelLogicWrapper.ViewContextsGroup,Enumerable.Empty<IModelViewContexts>());
        }
    }

    public class ActionExecutionContextGroupVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return !Equals(((IModelLogicRule) node).ModelLogicWrapper.ActionExecutionContextGroup,Enumerable.Empty<IModelActionExecutionContexts>());
        }
    }

}