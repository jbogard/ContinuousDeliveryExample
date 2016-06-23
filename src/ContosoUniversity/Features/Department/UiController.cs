﻿namespace ContosoUniversity.Features.Department
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Infrastructure;
    using MediatR;

    public class UiController : Controller
    {
        private readonly IMediator _mediator;

        public UiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ActionResult> Index()
        {
            var model = await _mediator.SendAsync(new Index.Query());

            return View(model);
        }

        public async Task<ActionResult> Details(Details.Query query)
        {
            var department = await _mediator.SendAsync(query);

            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        public ActionResult Create()
        {
            return View(new Create.Command());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Create.Command model)
        {
            _mediator.Send(model);

            return this.RedirectToActionJson(c => c.Index());
        }

        public async Task<ActionResult> Edit(Edit.Query query)
        {
            var department = await _mediator.SendAsync(query);

            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Edit.Command model)
        {
            await _mediator.SendAsync(model);

            return this.RedirectToActionJson(c => c.Index());
        }

        public async Task<ActionResult> Delete(Delete.Query query)
        {
            var model = await _mediator.SendAsync(query);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Delete.Command command)
        {
            await _mediator.SendAsync(command);

            return this.RedirectToActionJson(c => c.Index());
        }
    }
}