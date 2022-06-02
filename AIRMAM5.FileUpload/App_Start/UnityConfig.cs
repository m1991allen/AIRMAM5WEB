using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Services;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace AIRMAM5.FileUpload
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ISerilogService, SerilogService>();
            container.RegisterType<ISubjectService, SubjectService>();
            container.RegisterType<ITblLogService, TblLogService>();
            container.RegisterType<ICodeService, CodeService>();
            container.RegisterType<IFunctionsService, FunctionsService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}