using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plumber.Models
{
    public class ServiceModel
    {
        public int ServiceId { get; set; }

        public string ServiceName { get; set; }

        public string Description { get; set; }

        public List<ServiceModel> GetServices()
        {
            return new List<ServiceModel>()
            {
                new ServiceModel(){
                    ServiceId = 1,
                    ServiceName = "Sewer and Drain Cleaning",
                    Description = @"<p>
                        Stopped up drains never happen at a good time. <strong>1-800-PLUMBER&reg;</strong>
                        provides 24 hour emergency service when your kitchen sink, bath or main sewer line
                        backs up. When the drain lines back up in your business, you are out of business
                        and we understand that. It does not matter if it is a floor drain or your main sewer
                        line &ndash; we can help. Quickly and accurately we will determine the problem and
                        deliver the solution to keep you in business. Business owners, ask our 1-800-PLUMBER&reg;
                        Technician about our HomeGuard Plan to prevent the stoppages that slow your business
                        down.</p>"
                },
                new ServiceModel(){
                    ServiceId = 2,
                    ServiceName = "Water Jetting",
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
                new ServiceModel(){
                    ServiceId = 3,
                    ServiceName = "Sewer Camera Inspection",
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
                new ServiceModel(){
                    ServiceId = 4,
                    ServiceName = "Trenchless Pipe Repair",
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
                new ServiceModel(){
                    ServiceId = 5,
                    ServiceName = "Drain Maintenance Products",
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
                new ServiceModel(){
                    ServiceId = 6,
                    ServiceName = "Plumbing Repair or Replacement",
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
                new ServiceModel(){
                    ServiceId = 7,
                    ServiceName = "Water and Drain Line Location",
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
                new ServiceModel(){
                    ServiceId = 8,
                    ServiceName = "Backflow Devices",
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
                new ServiceModel(){
                    ServiceId = 9,
                    ServiceName = "Water Heaters",
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
                new ServiceModel(){
                    ServiceId = 10,
                    ServiceName = "Tankless Water Heater",
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
                new ServiceModel(){
                    ServiceId = 11,
                    ServiceName = "Faucets",
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
                new ServiceModel(){
                    ServiceId = 12,
                    ServiceName = "Sinks",
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
                new ServiceModel(){
                    ServiceId = 13,
                    ServiceName = "Toilets",
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
                new ServiceModel(){
                    ServiceId = 14,
                    ServiceName = "Heating and Cooling",
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
                new ServiceModel(){
                    ServiceId = 15,
                    ServiceName = "Air Purification",
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
                }
            };
        }
    }
}