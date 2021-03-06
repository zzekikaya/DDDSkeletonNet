// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using DDDSkeletonNET.Portal.ApplicationServices.Interfaces;
using DDDSkeletonNET.Portal.Domain.Customer;
using DDDSkeletonNET.Portal.Repository.Memory.Repositories;
using StructureMap;
using DDDSkeletonNET.Infrastructure.Common.Domain;
using DDDSkeletonNET.Infrastructure.Common.UnitOfWork;
using DDDSkeletonNET.Portal.Repository.Memory;
using DDDSkeletonNET.Portal.Repository.Memory.Database;
using DDDSkeletonNET.Infrastructure.Common.Caching;
using DDDSkeletonNET.Portal.ApplicationServices.Implementations;
using DDDSkeletonNET.Infrastructure.Common.Configuration;
using DDDSkeletonNET.Portal.Repository.Memory.DataContextFactoryOrm;
namespace DDDSkeletonNET.Portal.WebService.DependencyResolution
{
	public static class IoC
	{
		public static IContainer Initialize()
		{
			ObjectFactory.Initialize(x =>
						{
							x.Scan(scan =>
									{
										scan.TheCallingAssembly();
										scan.AssemblyContainingType<ICustomerRepository>();
										scan.AssemblyContainingType<CustomerRepository>();
										scan.AssemblyContainingType<ICustomerService>();
										scan.AssemblyContainingType<BusinessRule>();
										scan.WithDefaultConventions();
									});
							//x.For<IObjectContextFactory>().Use<LazySingletonObjectContextFactory>();
							x.For<IObjectContextFactory>().Use<HttpAwareOrmDataContextFactory>();							
							x.For<IUnitOfWork>().Use<InMemoryUnitOfWork>();							
							x.For<ICacheStorage>().Use<SystemRuntimeCacheStorage>();
							var customerService = x.For<ICustomerService>().Use<CustomerService>();
							x.For<ICustomerService>().Use<EnrichedCustomerService>().Ctor<ICustomerService>().Is(customerService);
							x.For<IConfigurationRepository>().Use<AppSettingsConfigurationRepository>();
						});
			ObjectFactory.AssertConfigurationIsValid();
			return ObjectFactory.Container;
		}
	}
}