﻿using LanguageGenerator.Core.SyntacticProperty;
using LanguageGenerator.Core.SyntacticProperty.ParentProperty;
using LanguageGenerator.Core.SyntacticUnit;
using NUnit.Framework;


namespace LanguageGenerator.Tests
{
    [TestFixture]
    class IntegrationTests_Of_BasicProperty
    {
        //DoesPropertyCanStartFrom tests are not using mocks, because mocking replaces their Equals method, which makes DoesPropertyCanStartFrom to work wrong 
        [Test]
        public void Does_DoesPropertyCanStartFrom_Works_If_Property_Includes_aProperty_in_CanStartFrom_Collection()
        {
            IProperty startFromThisProperty = new ParentProperty("startProperty");
            IProperty aProperty1 = new ParentProperty("aProperty1");
            IProperty aProperty2 = new ParentProperty("aProperty2");
            IProperty propertyWithCanStartFromCollection = new ParentProperty("propertyWithCanStartFromCollection");
            propertyWithCanStartFromCollection.StartsWithFrequencyFrom.Add(startFromThisProperty, 1);
            propertyWithCanStartFromCollection.StartsWithFrequencyFrom.Add(aProperty1, 1);
            propertyWithCanStartFromCollection.StartsWithFrequencyFrom.Add(aProperty2, 1);
            //Act 
            bool canStart = propertyWithCanStartFromCollection.DoesPropertyCanStartFrom(startFromThisProperty);
            //Assert
            Assert.That(canStart);
        }


        [Test]
        public void Does_DoesPropertyCanStartFrom_Returns_False_If_Property_Doesnt_Include_aProperty_in_CanStartFrom_Collection()
        {
            IProperty startFromThisProperty = new ParentProperty("startProperty");
            IProperty aProperty1 = new ParentProperty("aProperty1");
            IProperty aProperty2 = new ParentProperty("aProperty2");
            IProperty propertyWithCanStartFromCollection = new ParentProperty("propertyWithCanStartFromCollection");
            propertyWithCanStartFromCollection.StartsWithFrequencyFrom.Add(aProperty1, 1);
            propertyWithCanStartFromCollection.StartsWithFrequencyFrom.Add(aProperty2, 1);
            //Act 
            bool canStart = propertyWithCanStartFromCollection.DoesPropertyCanStartFrom(startFromThisProperty);
            //Assert
            Assert.That(!canStart);
        }


        [Test]
        public void Does_DoesPropertyCanStartFrom_Returns_False_On_EmptyProperty_Data()
        {
            IProperty startFromThisProperty = new ParentProperty("startProperty");
            IProperty propertyWithCanStartFromCollection = new ParentProperty("propertyWithEmptyCanStartFromCollection");
            //Act 
            bool canStart = propertyWithCanStartFromCollection.DoesPropertyCanStartFrom(startFromThisProperty);
            //Assert
            Assert.That(!canStart);
        }


        [Test]
        public void Does_DoesPropertyCanStartFrom_Works_If_CanStartFrom_Collection_Contains_Any_And_StartFrom_Properties()
        {
            IProperty startFromThisProperty = new ParentProperty("startProperty");
            IProperty aProperty1 = new ParentProperty("aProperty1");
            IProperty aProperty2 = new ParentProperty("aProperty2");
            IProperty propertyWithCanStartFromCollection = new ParentProperty("propertyWithCanStartFromCollection");
            propertyWithCanStartFromCollection.StartsWithFrequencyFrom.Add(startFromThisProperty, 1);
            propertyWithCanStartFromCollection.StartsWithFrequencyFrom.Add(aProperty1, 1);
            propertyWithCanStartFromCollection.StartsWithFrequencyFrom.Add(aProperty2, 1);
            propertyWithCanStartFromCollection.StartsWithFrequencyFrom.Add(BasicSyntacticUnitsSingleton.AnyProperty, 1);
            //Act 
            bool canStart = propertyWithCanStartFromCollection.DoesPropertyCanStartFrom(startFromThisProperty);
            //Assert
            Assert.That(canStart);
        }


        [Test]
        public void Does_DoesPropertyCanStartFrom_Works_If_CanStartFrom_Collection_Contains_Only_Any_Property()
        {
            IProperty startFromThisProperty = new ParentProperty("startProperty");
            IProperty aProperty1 = new ParentProperty("aProperty1");
            IProperty aProperty2 = new ParentProperty("aProperty2");
            IProperty propertyWithCanStartFromCollection = new ParentProperty("propertyWithCanStartFromCollection");
            propertyWithCanStartFromCollection.StartsWithFrequencyFrom.Add(aProperty1, 1);
            propertyWithCanStartFromCollection.StartsWithFrequencyFrom.Add(aProperty2, 1);
            propertyWithCanStartFromCollection.StartsWithFrequencyFrom.Add(BasicSyntacticUnitsSingleton.AnyProperty, 1);
            //Act 
            bool canStart = propertyWithCanStartFromCollection.DoesPropertyCanStartFrom(startFromThisProperty);
            //Assert
            Assert.That(canStart);
        }
    }
}