﻿namespace ContosoUniversity.Features.Student
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

        public ViewResult Index(Index.Query query)
        {
            var model = _mediator.Send(query);

            return View(model);
        }

        public async Task<ActionResult> Details(Details.Query query)
        {
            var model = await _mediator.SendAsync(query);

            return View(model);
        }

        public ActionResult Create()
        {
            return View(new Create.Command());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Create.Command command)
        {
            _mediator.Send(command);

            return this.RedirectToActionJson(c => c.Index(null));
        }


        public async Task<ActionResult> Edit(Edit.Query query)
        {
            var model = await _mediator.SendAsync(query);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Edit.Command command)
        {
            await _mediator.SendAsync(command);

            return this.RedirectToActionJson(c => c.Index(null));
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

            return this.RedirectToActionJson(c => c.Index(null));
        }
    }
}