﻿namespace ContosoUniversity.IntegrationTests
{
    using System;
    using DependencyResolution;
    using Ploeh.AutoFixture.Kernel;

    public class ContainerBuilder : ISpecimenBuilder
    {
        private readonly StructureMapDependencyScope scope;

        public ContainerBuilder(StructureMapDependencyScope scope)
        {
            this.scope = scope;
        }

        public object Create(object request, ISpecimenContext context)
        {
            var type = request as Type;

            if (type == null || type.IsPrimitive)
            {
                return new NoSpecimen(request);
            }

            var service = (scope.CurrentNestedContainer ?? scope.Container).TryGetInstance(type);

            return service ?? new NoSpecimen(request);
        }
    }
}