using System;

namespace DALI.PolicyRequirements.DomainModels
{
    public class PolicyRequirementMainPathModel
    {
        protected PolicyRequirementLocalAuthorityModel _LocalAuthority;
        protected PolicyRequirementChapterModel _Chapter;
        protected PolicyRequirementLevelModel _Level;
        protected PolicyRequirementAreaModel _Area;
        protected PolicyRequirementSubjectModel _Subject;
        protected PolicyRequirementLocationModel _Location;
        protected PolicyRequirementChildSubjectModel _ChildSubject;


        public Guid LocalAuthorityId
        {
            get
            {
                return _LocalAuthority != null ? _LocalAuthority.Id : default(Guid);
            }
        }

        public int ThemeId
        {
            get;
        }

        public int ChapterId
        {
            get
            {
                return _Chapter != null ? _Chapter.Id : default(int);
            }
        }

        public int LevelId
        {
            get
            {
                return _Level != null ? _Level.Id : default(int);
            }
        }

        public int AreaId
        {
            get
            {
                return _Area != null ? _Area.Id : default(int);
            }
        }

        public int SubjectId
        {
            get
            {
                return _Subject != null ? _Subject.Id : default(int);
            }
        }

        public int ChildSubjectId
        {
            get
            {
                return _ChildSubject != null ? _ChildSubject.Id : default(int);
            }
        }

        public Guid LocationId
        {
            get
            {
                return _Location != null ? _Location.Id : Guid.Empty;
            }
        }

        public virtual PolicyRequirementLocalAuthorityModel LocalAuthority
        {
            get
            {
                if (_LocalAuthority == null)
                    _LocalAuthority = new PolicyRequirementLocalAuthorityModel();

                return _LocalAuthority;
            }
            set
            {
                _LocalAuthority = value;
            }
        }

        public virtual PolicyRequirementChapterModel Chapter
        {
            get
            {
                if (_Chapter == null)
                    _Chapter = new PolicyRequirementChapterModel();

                return _Chapter;
            }
            set
            {
                _Chapter = value;
            }
        }

        public virtual PolicyRequirementLevelModel Level
        {
            get
            {
                if (_Level == null)
                    _Level = new PolicyRequirementLevelModel();

                return _Level;
            }
            set
            {
                _Level = value;
            }
        }

        public virtual PolicyRequirementLocationModel Location
        {
            get
            {
                if (_Location == null)
                    _Location = new PolicyRequirementLocationModel();

                return _Location;
            }
            set
            {
                _Location = value;
            }
        }

        public virtual PolicyRequirementAreaModel Area
        {
            get
            {
                if (_Area == null)
                    _Area = new PolicyRequirementAreaModel();

                return _Area;
            }
            set
            {
                _Area = value;
            }
        }

        public virtual PolicyRequirementSubjectModel Subject
        {
            get
            {
                if (_Subject == null)
                    _Subject = new PolicyRequirementSubjectModel();

                return _Subject;
            }
            set
            {
                _Subject = value;
            }
        }

        public virtual PolicyRequirementChildSubjectModel ChildSubject
        {
            get
            {
                if (_ChildSubject == null)
                    _ChildSubject = new PolicyRequirementChildSubjectModel();

                return _ChildSubject;
            }
            set
            {
                _ChildSubject = value;
            }
        }

        public string SemiPath
        {
            get
            {
                return string.Format("{0}|{1}|{2}|{3}", Chapter.FullChapterDescription, Level.Name, Location.Description, Area.Description);
            }
        }

        public string SemiPathKey
        {
            get
            {
                return string.Format("{0}_{1}_{2}_{3}_{4}", Chapter.Id, Level.Id, Location.Id, Area.Id, Subject.Id);
            }
        }

        public string Path
        {
            get
            {
                return string.Format("{0}|{1}|{2}|{3}|{4}|{5}", Chapter.FullChapterDescription, Level.Name, Location.Description, Area.Description, Subject.Description, ChildSubject.Description);
            }
        }
    }
}
