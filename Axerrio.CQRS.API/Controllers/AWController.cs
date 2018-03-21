﻿using Axerrio.CQRS.API.Application.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Controllers
{
    public class AWController: Controller
    {
        private readonly AWContext _context;
        private readonly IEdmModel _model;

        public AWController(AWContext context, IEdmModel model)
        {
            _context = context;
            _model = model;
        }

        [HttpGet("odata/edmmodel")]
        public IActionResult GetEdmModel()
        {
            var custSet = _model.EntityContainer.FindEntitySet("Customers");
            var custType = custSet.EntityType();

            var json = JsonConvert.SerializeObject(custType, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Ok(json);
        }

        [HttpGet("aw/productpictures")]
        public IActionResult GetProductPictures()
        {
            var rootQuery = _context.ProductPictures;

            var pictures = rootQuery.ToList();

            return Ok(pictures);
        }

        [HttpGet("aw/productpictures2")]
        public IActionResult GetProductPictures2()
        {
            var rootQuery = _context.ProductPictures2;

            var pictures = rootQuery.ToList();

            return Ok(pictures);
        }

        [HttpGet("aw/productpictures2/{Id}")]
        public IActionResult GetProductPicturesByID(int Id) //706
        {
            var outer = _context.Products
                .Where(p => p.ProductID == Id);

            var inner = _context.ProductPictures2;

            //var query = outer.Join
            //    (
            //        inner: inner,
            //        outerKeySelector: p => p.ProductID,
            //        innerKeySelector: pp => pp.ProductID,
            //        resultSelector: (p, pp) => new { Product = p, Pictures = pp}
            //    );

            var query = outer.GroupJoin
                (
                    inner: inner,
                    outerKeySelector: product => product.ProductID,
                    innerKeySelector: productPictures => productPictures.ProductID,
                    //resultSelector: (product, productPictures) => new { Product = product, Pictures = productPictures.Select(pp => pp.Picture).ToList() }
                    resultSelector: (product, productPictures) => new { Product = product, Pictures = productPictures }
                );

            var pictures = query.ToList();

            return Ok(pictures);
        }
    }
}
