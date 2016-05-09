using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plumber.Models
{
    public class ManagementModel
    {

        public int ManagementId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string AlternativeText { get; set; }

        public List<ManagementModel> GetManagements()
        {
            return new List<ManagementModel>()
            {
                new ManagementModel(){
                    ManagementId = 1,
                    FirstName = "Jason",
                    LastName = "Melton",
                    Title = "President and CEO",
                    Description = @"<p>
                                    Jason Melton serves as President and CEO of 1-800-PLUMBER. Mr. Melton has served
                                    as a senior executive with Taylor Interests and several of its operating businesses,
                                    holding the positions of Chief Executive Officer, Chief Operating Officer, Managing
                                    Director and Senior Advisor. These businesses have included technology, professional
                                    services, strategic consulting, distribution and natural resources.
                                </p>
                                <p>
                                    Prior to joining Taylor Interests, Mr. Melton served as Chief Financial Officer
                                    of Valencia Group, a developer and operator of upscale hotels across North America.
                                    In addition to leading Valencia's finance and accounting groups, he was responsible
                                    for the commercial and legal compliance, risk management, human resource and information
                                    technology functions of the company.
                                </p>
                                <p>
                                    Before his tenure at Valencia, Mr. Melton led the worldwide business development
                                    and merger and acquisition efforts of ASCO, an $800 million oilfield services company
                                    with approximately 3,000 employees in six countries. Mr. Melton has also worked
                                    as an investment banker at Donaldson, Lufkin &amp; Jenrette where he advised on
                                    numerous deals in a variety of energy sectors, including oil and gas exploration
                                    and production, oilfield services, oil and gas refining, storage and delivery, and
                                    power generation and distribution.</p>
                                <p>
                                    Mr. Melton grew up in a state park in west Texas and attended Princeton University
                                    where he received an A.B. in Economics.</p>",
                    ImageUrl = "~/Images/jason.jpg",
                    AlternativeText = "CEO"
                },
                new ManagementModel(){
                    ManagementId = 2,
                    FirstName = "Jennifer",
                    LastName = "Collins",
                    Title = "Director of Marketing",
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
                    ImageUrl = "~/Images/no-image.jpg",
                    AlternativeText = ""
                },
                new ManagementModel(){
                    ManagementId = 3,
                    FirstName = "Robin-Jan",
                    LastName = "De Lange",
                    Title = "Chief Information Officer",
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
                    ImageUrl = "~/Images/no-image.jpg",
                    AlternativeText = ""
                },
                new ManagementModel(){
                    ManagementId = 4,
                    FirstName = "James",
                    LastName = "Clarke",
                    Title = "Integration Manager",
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
                    ImageUrl = "~/Images/no-image.jpg",
                    AlternativeText = ""
                },
                new ManagementModel(){
                    ManagementId = 5,
                    FirstName = "Jim",
                    LastName = "Hughes",
                    Title = "Director of Training & Technical Development",
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
                    ImageUrl = "~/Images/no-image.jpg",
                    AlternativeText = ""
                },
                new ManagementModel(){
                    ManagementId = 6,
                    FirstName = "Jane",
                    LastName = "Byrns",
                    Title = "Controller",
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
                    ImageUrl = "~/Images/no-image.jpg",
                    AlternativeText = ""
                },
                new ManagementModel(){
                    ManagementId = 7,
                    FirstName = "Les",
                    LastName = "Strech",
                    Title = "Director of Franchise Development",
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
                    ImageUrl = "~/Images/no-image.jpg",
                    AlternativeText = ""
                },
                new ManagementModel(){
                    ManagementId = 8,
                    FirstName = "Patricia",
                    LastName = "Martin",
                    Title = "Service Center Manager",
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
                    ImageUrl = "~/Images/no-image.jpg",
                    AlternativeText = ""
                }
            };
        }
    }
}