using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plumber.Models
{
    public class SpecialOfferModel
    {
        public int SpecialOfferId { get; set; }

        public string SpecialOfferName { get; set; }

        public string Description { get; set; }

        public List<SpecialOfferModel> GetSpecialOffers()
        {
            return new List<SpecialOfferModel>()
            {
                new SpecialOfferModel(){
                    SpecialOfferId = 1,
                    SpecialOfferName = "Air Conditioning",
                    Description = @"<p>
                                        Amarillo, Adrian, Borger, Canyon, Claude, Fritch, Groom, Happy, Hereford, Masterson,
                                        Pampa, Panhandle, Skellytown, Vega, Wayside, White Deer, you do not have to be hot
                                        this year! <strong>1-800-PLUMBER</strong> can take care of all your cooling needs.
                                        Annual A/C tune ups, complete repairs, or new installations are performed with ease
                                        by <strong>1-800-PLUMBER</strong>.</p>
                                    <p>
                                        Talk to us today about our new energy efficient air conditioning units. These units
                                        will save on your electric bill, plus keep you nice and cool, even on the hottest
                                        of days. Call today for <strong>1-800-PLUMBER</strong> to provide a free evaluation
                                        and estimate.</p>"
                },
                new SpecialOfferModel(){
                    SpecialOfferId = 2,
                    SpecialOfferName = "Backflow Preventers",
                    Description = @"<p>
                                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit, 
                                        sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna 
                                        aliquam erat volutpat. Ut wisi enim ad minim veniam, quis 
                                        nostrud exerci tation ullamcorper suscipit lobortis nisl ut 
                                        aliquip ex ea commodo consequat. Duis autem vel eum iriure 
                                        dolor in hendrerit in vulputate velit esse molestie consequat, 
                                        vel illum dolore eu feugiat nulla facilisis at vero eros et 
                                        accumsan et iusto odio dignissim qui blandit praesent luptatum 
                                        zzril delenit augue duis dolore te feugait nulla facilisi.
                                    </p>
                                    <p>
                                        Nam liber tempor cum soluta nobis eleifend option congue 
                                        nihil imperdiet doming id quod mazim placerat facer possim 
                                        assum. Typi non habent claritatem insitam; est usus legentis 
                                        in iis qui facit eorum claritatem. Investigationes demonstraverunt 
                                        lectores legere me lius quod ii legunt saepius. 
                                        Claritas est etiam processus dynamicus, qui sequitur 
                                        mutationem consuetudium lectorum. Mirum est notare 
                                        quam littera gothica, quam nunc putamus parum claram, 
                                        anteposuerit litterarum formas humanitatis per seacula 
                                        quarta decima et quinta decima. Eodem modo typi, qui 
                                        nunc nobis videntur parum clari, fiant sollemnes in futurum.
                                    </p>"
                },
                new SpecialOfferModel(){
                    SpecialOfferId = 3,
                    SpecialOfferName = "Drain Cleaning",
                    Description = @"<p>
                                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit, 
                                        sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna 
                                        aliquam erat volutpat. Ut wisi enim ad minim veniam, quis 
                                        nostrud exerci tation ullamcorper suscipit lobortis nisl ut 
                                        aliquip ex ea commodo consequat. Duis autem vel eum iriure 
                                        dolor in hendrerit in vulputate velit esse molestie consequat, 
                                        vel illum dolore eu feugiat nulla facilisis at vero eros et 
                                        accumsan et iusto odio dignissim qui blandit praesent luptatum 
                                        zzril delenit augue duis dolore te feugait nulla facilisi.
                                    </p>
                                    <p>
                                        Nam liber tempor cum soluta nobis eleifend option congue 
                                        nihil imperdiet doming id quod mazim placerat facer possim 
                                        assum. Typi non habent claritatem insitam; est usus legentis 
                                        in iis qui facit eorum claritatem. Investigationes demonstraverunt 
                                        lectores legere me lius quod ii legunt saepius. 
                                        Claritas est etiam processus dynamicus, qui sequitur 
                                        mutationem consuetudium lectorum. Mirum est notare 
                                        quam littera gothica, quam nunc putamus parum claram, 
                                        anteposuerit litterarum formas humanitatis per seacula 
                                        quarta decima et quinta decima. Eodem modo typi, qui 
                                        nunc nobis videntur parum clari, fiant sollemnes in futurum.
                                    </p>"
                },
                new SpecialOfferModel(){
                    SpecialOfferId = 4,
                    SpecialOfferName = "Drain Line and Waste Line Location",
                    Description = @"<p>
                                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit, 
                                        sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna 
                                        aliquam erat volutpat. Ut wisi enim ad minim veniam, quis 
                                        nostrud exerci tation ullamcorper suscipit lobortis nisl ut 
                                        aliquip ex ea commodo consequat. Duis autem vel eum iriure 
                                        dolor in hendrerit in vulputate velit esse molestie consequat, 
                                        vel illum dolore eu feugiat nulla facilisis at vero eros et 
                                        accumsan et iusto odio dignissim qui blandit praesent luptatum 
                                        zzril delenit augue duis dolore te feugait nulla facilisi.
                                    </p>
                                    <p>
                                        Nam liber tempor cum soluta nobis eleifend option congue 
                                        nihil imperdiet doming id quod mazim placerat facer possim 
                                        assum. Typi non habent claritatem insitam; est usus legentis 
                                        in iis qui facit eorum claritatem. Investigationes demonstraverunt 
                                        lectores legere me lius quod ii legunt saepius. 
                                        Claritas est etiam processus dynamicus, qui sequitur 
                                        mutationem consuetudium lectorum. Mirum est notare 
                                        quam littera gothica, quam nunc putamus parum claram, 
                                        anteposuerit litterarum formas humanitatis per seacula 
                                        quarta decima et quinta decima. Eodem modo typi, qui 
                                        nunc nobis videntur parum clari, fiant sollemnes in futurum.
                                    </p>"
                },
                new SpecialOfferModel(){
                    SpecialOfferId = 5,
                    SpecialOfferName = "Heating",
                    Description = @"<p>
                                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit, 
                                        sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna 
                                        aliquam erat volutpat. Ut wisi enim ad minim veniam, quis 
                                        nostrud exerci tation ullamcorper suscipit lobortis nisl ut 
                                        aliquip ex ea commodo consequat. Duis autem vel eum iriure 
                                        dolor in hendrerit in vulputate velit esse molestie consequat, 
                                        vel illum dolore eu feugiat nulla facilisis at vero eros et 
                                        accumsan et iusto odio dignissim qui blandit praesent luptatum 
                                        zzril delenit augue duis dolore te feugait nulla facilisi.
                                    </p>
                                    <p>
                                        Nam liber tempor cum soluta nobis eleifend option congue 
                                        nihil imperdiet doming id quod mazim placerat facer possim 
                                        assum. Typi non habent claritatem insitam; est usus legentis 
                                        in iis qui facit eorum claritatem. Investigationes demonstraverunt 
                                        lectores legere me lius quod ii legunt saepius. 
                                        Claritas est etiam processus dynamicus, qui sequitur 
                                        mutationem consuetudium lectorum. Mirum est notare 
                                        quam littera gothica, quam nunc putamus parum claram, 
                                        anteposuerit litterarum formas humanitatis per seacula 
                                        quarta decima et quinta decima. Eodem modo typi, qui 
                                        nunc nobis videntur parum clari, fiant sollemnes in futurum.
                                    </p>"
                },
                new SpecialOfferModel(){
                    SpecialOfferId = 6,
                    SpecialOfferName = "Leak Detection",
                    Description = @"<p>
                                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit, 
                                        sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna 
                                        aliquam erat volutpat. Ut wisi enim ad minim veniam, quis 
                                        nostrud exerci tation ullamcorper suscipit lobortis nisl ut 
                                        aliquip ex ea commodo consequat. Duis autem vel eum iriure 
                                        dolor in hendrerit in vulputate velit esse molestie consequat, 
                                        vel illum dolore eu feugiat nulla facilisis at vero eros et 
                                        accumsan et iusto odio dignissim qui blandit praesent luptatum 
                                        zzril delenit augue duis dolore te feugait nulla facilisi.
                                    </p>
                                    <p>
                                        Nam liber tempor cum soluta nobis eleifend option congue 
                                        nihil imperdiet doming id quod mazim placerat facer possim 
                                        assum. Typi non habent claritatem insitam; est usus legentis 
                                        in iis qui facit eorum claritatem. Investigationes demonstraverunt 
                                        lectores legere me lius quod ii legunt saepius. 
                                        Claritas est etiam processus dynamicus, qui sequitur 
                                        mutationem consuetudium lectorum. Mirum est notare 
                                        quam littera gothica, quam nunc putamus parum claram, 
                                        anteposuerit litterarum formas humanitatis per seacula 
                                        quarta decima et quinta decima. Eodem modo typi, qui 
                                        nunc nobis videntur parum clari, fiant sollemnes in futurum.
                                    </p>"
                },
                new SpecialOfferModel(){
                    SpecialOfferId = 7,
                    SpecialOfferName = "Plumbing Repair and Replacement",
                    Description = @"<p>
                                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit, 
                                        sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna 
                                        aliquam erat volutpat. Ut wisi enim ad minim veniam, quis 
                                        nostrud exerci tation ullamcorper suscipit lobortis nisl ut 
                                        aliquip ex ea commodo consequat. Duis autem vel eum iriure 
                                        dolor in hendrerit in vulputate velit esse molestie consequat, 
                                        vel illum dolore eu feugiat nulla facilisis at vero eros et 
                                        accumsan et iusto odio dignissim qui blandit praesent luptatum 
                                        zzril delenit augue duis dolore te feugait nulla facilisi.
                                    </p>
                                    <p>
                                        Nam liber tempor cum soluta nobis eleifend option congue 
                                        nihil imperdiet doming id quod mazim placerat facer possim 
                                        assum. Typi non habent claritatem insitam; est usus legentis 
                                        in iis qui facit eorum claritatem. Investigationes demonstraverunt 
                                        lectores legere me lius quod ii legunt saepius. 
                                        Claritas est etiam processus dynamicus, qui sequitur 
                                        mutationem consuetudium lectorum. Mirum est notare 
                                        quam littera gothica, quam nunc putamus parum claram, 
                                        anteposuerit litterarum formas humanitatis per seacula 
                                        quarta decima et quinta decima. Eodem modo typi, qui 
                                        nunc nobis videntur parum clari, fiant sollemnes in futurum.
                                    </p>"
                },
                new SpecialOfferModel(){
                    SpecialOfferId = 8,
                    SpecialOfferName = "Sewer Camera Inspection",
                    Description = @"<p>
                                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit, 
                                        sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna 
                                        aliquam erat volutpat. Ut wisi enim ad minim veniam, quis 
                                        nostrud exerci tation ullamcorper suscipit lobortis nisl ut 
                                        aliquip ex ea commodo consequat. Duis autem vel eum iriure 
                                        dolor in hendrerit in vulputate velit esse molestie consequat, 
                                        vel illum dolore eu feugiat nulla facilisis at vero eros et 
                                        accumsan et iusto odio dignissim qui blandit praesent luptatum 
                                        zzril delenit augue duis dolore te feugait nulla facilisi.
                                    </p>
                                    <p>
                                        Nam liber tempor cum soluta nobis eleifend option congue 
                                        nihil imperdiet doming id quod mazim placerat facer possim 
                                        assum. Typi non habent claritatem insitam; est usus legentis 
                                        in iis qui facit eorum claritatem. Investigationes demonstraverunt 
                                        lectores legere me lius quod ii legunt saepius. 
                                        Claritas est etiam processus dynamicus, qui sequitur 
                                        mutationem consuetudium lectorum. Mirum est notare 
                                        quam littera gothica, quam nunc putamus parum claram, 
                                        anteposuerit litterarum formas humanitatis per seacula 
                                        quarta decima et quinta decima. Eodem modo typi, qui 
                                        nunc nobis videntur parum clari, fiant sollemnes in futurum.
                                    </p>"
                },
                new SpecialOfferModel(){
                    SpecialOfferId = 9,
                    SpecialOfferName = "Slab Leaks and Yard Leaks",
                    Description = @"<p>
                                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit, 
                                        sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna 
                                        aliquam erat volutpat. Ut wisi enim ad minim veniam, quis 
                                        nostrud exerci tation ullamcorper suscipit lobortis nisl ut 
                                        aliquip ex ea commodo consequat. Duis autem vel eum iriure 
                                        dolor in hendrerit in vulputate velit esse molestie consequat, 
                                        vel illum dolore eu feugiat nulla facilisis at vero eros et 
                                        accumsan et iusto odio dignissim qui blandit praesent luptatum 
                                        zzril delenit augue duis dolore te feugait nulla facilisi.
                                    </p>
                                    <p>
                                        Nam liber tempor cum soluta nobis eleifend option congue 
                                        nihil imperdiet doming id quod mazim placerat facer possim 
                                        assum. Typi non habent claritatem insitam; est usus legentis 
                                        in iis qui facit eorum claritatem. Investigationes demonstraverunt 
                                        lectores legere me lius quod ii legunt saepius. 
                                        Claritas est etiam processus dynamicus, qui sequitur 
                                        mutationem consuetudium lectorum. Mirum est notare 
                                        quam littera gothica, quam nunc putamus parum claram, 
                                        anteposuerit litterarum formas humanitatis per seacula 
                                        quarta decima et quinta decima. Eodem modo typi, qui 
                                        nunc nobis videntur parum clari, fiant sollemnes in futurum.
                                    </p>"
                },
                new SpecialOfferModel(){
                    SpecialOfferId = 10,
                    SpecialOfferName = "Tankless Water Heaters",
                    Description = @"<p>
                                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit, 
                                        sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna 
                                        aliquam erat volutpat. Ut wisi enim ad minim veniam, quis 
                                        nostrud exerci tation ullamcorper suscipit lobortis nisl ut 
                                        aliquip ex ea commodo consequat. Duis autem vel eum iriure 
                                        dolor in hendrerit in vulputate velit esse molestie consequat, 
                                        vel illum dolore eu feugiat nulla facilisis at vero eros et 
                                        accumsan et iusto odio dignissim qui blandit praesent luptatum 
                                        zzril delenit augue duis dolore te feugait nulla facilisi.
                                    </p>
                                    <p>
                                        Nam liber tempor cum soluta nobis eleifend option congue 
                                        nihil imperdiet doming id quod mazim placerat facer possim 
                                        assum. Typi non habent claritatem insitam; est usus legentis 
                                        in iis qui facit eorum claritatem. Investigationes demonstraverunt 
                                        lectores legere me lius quod ii legunt saepius. 
                                        Claritas est etiam processus dynamicus, qui sequitur 
                                        mutationem consuetudium lectorum. Mirum est notare 
                                        quam littera gothica, quam nunc putamus parum claram, 
                                        anteposuerit litterarum formas humanitatis per seacula 
                                        quarta decima et quinta decima. Eodem modo typi, qui 
                                        nunc nobis videntur parum clari, fiant sollemnes in futurum.
                                    </p>"
                },
                new SpecialOfferModel(){
                    SpecialOfferId = 11,
                    SpecialOfferName = "Water Heaters",
                    Description = @"<p>
                                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit, 
                                        sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna 
                                        aliquam erat volutpat. Ut wisi enim ad minim veniam, quis 
                                        nostrud exerci tation ullamcorper suscipit lobortis nisl ut 
                                        aliquip ex ea commodo consequat. Duis autem vel eum iriure 
                                        dolor in hendrerit in vulputate velit esse molestie consequat, 
                                        vel illum dolore eu feugiat nulla facilisis at vero eros et 
                                        accumsan et iusto odio dignissim qui blandit praesent luptatum 
                                        zzril delenit augue duis dolore te feugait nulla facilisi.
                                    </p>
                                    <p>
                                        Nam liber tempor cum soluta nobis eleifend option congue 
                                        nihil imperdiet doming id quod mazim placerat facer possim 
                                        assum. Typi non habent claritatem insitam; est usus legentis 
                                        in iis qui facit eorum claritatem. Investigationes demonstraverunt 
                                        lectores legere me lius quod ii legunt saepius. 
                                        Claritas est etiam processus dynamicus, qui sequitur 
                                        mutationem consuetudium lectorum. Mirum est notare 
                                        quam littera gothica, quam nunc putamus parum claram, 
                                        anteposuerit litterarum formas humanitatis per seacula 
                                        quarta decima et quinta decima. Eodem modo typi, qui 
                                        nunc nobis videntur parum clari, fiant sollemnes in futurum.
                                    </p>"
                },
                new SpecialOfferModel(){
                    SpecialOfferId = 12,
                    SpecialOfferName = "Water Jetting",
                    Description = @"<p>
                                        Lorem ipsum dolor sit amet, consectetuer adipiscing elit, 
                                        sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna 
                                        aliquam erat volutpat. Ut wisi enim ad minim veniam, quis 
                                        nostrud exerci tation ullamcorper suscipit lobortis nisl ut 
                                        aliquip ex ea commodo consequat. Duis autem vel eum iriure 
                                        dolor in hendrerit in vulputate velit esse molestie consequat, 
                                        vel illum dolore eu feugiat nulla facilisis at vero eros et 
                                        accumsan et iusto odio dignissim qui blandit praesent luptatum 
                                        zzril delenit augue duis dolore te feugait nulla facilisi.
                                    </p>
                                    <p>
                                        Nam liber tempor cum soluta nobis eleifend option congue 
                                        nihil imperdiet doming id quod mazim placerat facer possim 
                                        assum. Typi non habent claritatem insitam; est usus legentis 
                                        in iis qui facit eorum claritatem. Investigationes demonstraverunt 
                                        lectores legere me lius quod ii legunt saepius. 
                                        Claritas est etiam processus dynamicus, qui sequitur 
                                        mutationem consuetudium lectorum. Mirum est notare 
                                        quam littera gothica, quam nunc putamus parum claram, 
                                        anteposuerit litterarum formas humanitatis per seacula 
                                        quarta decima et quinta decima. Eodem modo typi, qui 
                                        nunc nobis videntur parum clari, fiant sollemnes in futurum.
                                    </p>"
                },
            };
        }
    }
}