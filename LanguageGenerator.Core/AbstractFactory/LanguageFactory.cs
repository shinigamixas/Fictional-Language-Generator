﻿using System;
using LanguageGenerator.Core.Repository;
using LanguageGenerator.Core.SyntacticProperty;
using LanguageGenerator.Core.SyntacticProperty.ParentProperty;
using LanguageGenerator.Core.SyntacticProperty.RootProperty;
using LanguageGenerator.Core.SyntacticUnit;
using LanguageGenerator.Core.SyntacticUnit.ParentSU;
using LanguageGenerator.Core.SyntacticUnit.RootSU;


namespace LanguageGenerator.Core.AbstractFactory
{
    public class LanguageFactory : ILanguageFactory
    {
        private IRootProperty _lastCreatedProperty;


        public LanguageFactory(ISyntacticUnitRepository repository)
        {
            Repository = repository;
        }


        public LanguageFactory() : this(new SyntacticUnitRepository())
        {
        }


        public ISyntacticUnitRepository Repository { get; }


        public IRootProperty CreateRootProperty(string propertyName)
        {
            IRootProperty rootProperty = new RootProperty(propertyName);
            SetLastCreatedProperty(rootProperty);
            Repository.Properties.Add(rootProperty);
            return rootProperty;
        }


        public IParentProperty CreateParentProperty(string propertyName)
        {
            IParentProperty parentProperty = new ParentProperty(propertyName);
            Repository.Properties.Add(parentProperty);
            return parentProperty;
        }


        public IRootSU CreateRootSyntacticUnit(string stringRepresentation, IRootProperty itsProperty, int frequency = 100)
        {
            IRootSU rootSyntacticUnit = new RootSU(stringRepresentation, frequency, itsProperty);
            Repository.SyntacticUnits.Add(rootSyntacticUnit);
            return rootSyntacticUnit;
        }


        public IRootSU CreateRootSyntacticUnit(string stringRepresentation, string itsPropertyName, int frequency = 100)
        {
            try
            {
                IRootProperty rootProperty = (IRootProperty) Repository.GetPropertyWithName(itsPropertyName);
                return CreateRootSyntacticUnit(stringRepresentation, rootProperty, frequency);
            }
            catch (InvalidOperationException exception)
            {
                throw new PropertyNotExistsInRepositoryException(
                    itsPropertyName + " property not found in repository on creating root syntactic unit.", exception);
            }
        }


        public IRootSU CreateRootSyntacticUnitWithLastCreatedProperty(string stringRepresentation, int frequency = 100)
        {
            if(_lastCreatedProperty == null)
                throw new NullReferenceException("No property created before using this method.");
            return CreateRootSyntacticUnit(stringRepresentation, _lastCreatedProperty, frequency);
        }


        public IParentSU CreateParentSyntacticUnit(string itsPropertyName, int frequency = 100)
        {
            try
            {
                IParentProperty parentProperty = (IParentProperty) Repository.GetPropertyWithName(itsPropertyName);
                return CreateParentSyntacticUnit(parentProperty, frequency);
            }
            catch (InvalidOperationException exception)
            {
                throw new PropertyNotExistsInRepositoryException(
                    itsPropertyName + " property not found in repository on creating parent syntactic unit.", exception);
            }
        }


        public IParentSU CreateParentSyntacticUnit(IParentProperty itsProperty, int frequency = 100)
        {
            IParentSU parentSu = new ParentSU(frequency, itsProperty);
            Repository.SyntacticUnits.Add(parentSu);
            return parentSu;
        }


        private void SetLastCreatedProperty(IRootProperty property)
        {
            _lastCreatedProperty = property;
        }
    }
}