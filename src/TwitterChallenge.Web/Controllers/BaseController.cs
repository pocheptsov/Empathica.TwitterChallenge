using System;
using System.Web.Mvc;
using Empathica.TwitterChallenge.Db;
using Empathica.TwitterChallenge.Web.Extensions;
using Empathica.TwitterChallenge.Wires.Config;

namespace Empathica.TwitterChallenge.Web.Controllers
{
    public class BaseController : Controller
    {
        //delayed initialization when infrastructure will be ready
        private readonly Lazy<Container> _container;
        private readonly Lazy<ISessionAdapter> _sessionAdapter;
        private IRepository _repository;

        public BaseController()
        {
            _container = new Lazy<Container>(
                () => ((MvcApplication) HttpContext.ApplicationInstance)
                          .AdapterInstance
                          .Value
                          .Container);
            _sessionAdapter = new Lazy<ISessionAdapter>(
                () => new SessionAdapter(HttpContext));
        }

        //ctor for mocking
        internal BaseController(Container container, ISessionAdapter sessionAdapter)
        {
            _container = new Lazy<Container>(() => container);
            _sessionAdapter = new Lazy<ISessionAdapter>(() => sessionAdapter);
        }

        protected Container Container
        {
            get { return _container.Value; }
        }

        protected IRepository Repository
        {
            get
            {
                if (_repository == null)
                {
                    _repository = Container.RepositoryFactory();
                }
                return _repository;
            }
        }

        //add abstraction layer over standard session
        public new ISessionAdapter Session
        {
            get { return _sessionAdapter.Value; }
        }
    }
}