using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plumber.Models
{
    public class LocationModel
    {
        public int LocationId { get; set; }

        public string Description { get; set; }

        public string LocationName { get; set; }

        public string State { get; set; }

        public string Address { get; set; }

        public string ZipCode { get; set; }

        public string LocalPhoneNumber { get; set; }

        public List<LocationModel> GetLocations()
        {
            return new List<LocationModel>(){
                new LocationModel(){
                    LocationId = 1,
                    Description = @"<p>
                                        For over 15 years, the Shreiner family have served the plumbing, heating and cooling
                                        needs of Amarillo and the surrounding area with their business Shreiner Plumbing.
                                        In August 2011, the Shreiners chose to join 1-800-PLUMBER to provide a level of
                                        customer service unrivaled in the Amarillo community.</p>
                                    <p>
                                        1-800-PLUMBER of Amarillo specializes in plumbing, draining cleaning, sewer repair,
                                        as well as heating and air conditioning repair and installation. Call today for
                                        a free home services inspection and free estimates.
                                    </p>",
                    LocationName = "Amarillo",
                    State = "TX",
                    Address = "1751 SE 16th St",
                    ZipCode = "79102",
                    LocalPhoneNumber = "806-622-3862"
                },
                new LocationModel(){
                    LocationId = 2,
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
                                    </p>",
                    LocationName = "Atlanta",
                    State = "GA",
                    Address = "",
                    ZipCode = "",
                    LocalPhoneNumber = ""
                },
                new LocationModel(){
                    LocationId = 3,
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
                                    </p>",
                    LocationName = "Augusta",
                    State = "GA West",
                    Address = "",
                    ZipCode = "",
                    LocalPhoneNumber = ""
                },
                new LocationModel(){
                    LocationId = 4,
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
                                    </p>",
                    LocationName = "Dallas",
                    State = "TX",
                    Address = "",
                    ZipCode = "",
                    LocalPhoneNumber = ""
                },
                new LocationModel(){
                    LocationId = 5,
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
                                    </p>",
                    LocationName = "Danbury",
                    State = "CT",
                    Address = "",
                    ZipCode = "",
                    LocalPhoneNumber = ""
                },
                new LocationModel(){
                    LocationId = 6,
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
                                    </p>",
                    LocationName = "Greenville",
                    State = "SC",
                    Address = "",
                    ZipCode = "",
                    LocalPhoneNumber = ""
                },
                new LocationModel(){
                    LocationId = 7,
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
                                    </p>",
                    LocationName = "Mesa",
                    State = "AZ",
                    Address = "",
                    ZipCode = "",
                    LocalPhoneNumber = ""
                },
                new LocationModel(){
                    LocationId = 8,
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
                                    </p>",
                    LocationName = "Pearland",
                    State = "Houston South",
                    Address = "",
                    ZipCode = "",
                    LocalPhoneNumber = ""
                },
                new LocationModel(){
                    LocationId = 9,
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
                                    </p>",
                    LocationName = "Stamford",
                    State = "CT",
                    Address = "",
                    ZipCode = "",
                    LocalPhoneNumber = ""
                },
            };
        }

    }

}