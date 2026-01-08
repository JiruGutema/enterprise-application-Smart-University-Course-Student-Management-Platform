using SmartUniversity.Modules.Courses.Domain.Aggregates;
using SmartUniversity.Modules.Courses.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartUniversity.Modules.Courses.Application.Services
{
    public class CourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        // Create a new course
        public async Task<Course> CreateCourseAsync(Course course)
        {
            if (course.Code == null || string.IsNullOrWhiteSpace(course.Code.Value))
                throw new ArgumentException("Course code cannot be empty");

            if (string.IsNullOrWhiteSpace(course.Title))
                throw new ArgumentException("Course title cannot be empty");

            await _courseRepository.AddAsync(course);
            return course;
        }

        // Update course details
        public async Task UpdateCourseAsync(Course course)
        {
            await _courseRepository.UpdateAsync(course);
        }

        // Publish a course
        public async Task PublishCourseAsync(Course course)
        {
            course.Publish(); // domain method sets Status, UpdatedAt, and raises event
            await _courseRepository.UpdateAsync(course);
        }

        // Unpublish a course
        public async Task UnpublishCourseAsync(Course course)
        {
            course.Unpublish(); // domain method
            await _courseRepository.UpdateAsync(course);
        }

        // Soft-delete a course
        public async Task DeleteCourseAsync(Course course)
        {
            // Your Course entity currently does NOT have IsDeleted.
            // You can either add a private bool _isDeleted with a public getter,
            // or create a domain method:
            throw new NotImplementedException("Soft delete not implemented in domain.");
        }

        // Optional: validate prerequisites for a student
        public async Task<bool> ValidatePrerequisitesAsync(Guid studentId, IEnumerable<string> prerequisiteCodes)
        {
            return true; // stub, replace with real logic
        }
    }
}
