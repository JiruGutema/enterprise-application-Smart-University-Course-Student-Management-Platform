# Group Members

| **Full Name** | **ID** |
| -------------- | ------------ |
| Jiru Gutema    | UGR/5902/15  |
| Anansi Sime    | UGR/9691/15  |
| venusia biruk  | ugr/3522/15  |
| Yodit Debebe   | UGR/8065/15  |
| Tsion Shimelis | UGR/0654/15  |

# **Project Proposal**

**Project Name:** Smart University Course & Student Management Platform (with AI Academic Assistant)

## **ABSTRACT**

The Smart University Course & Student Management Platform addresses inefficiencies in current academic management systems by providing a unified platform for course administration, enrollment, content delivery, grading, and academic assistance. Traditional university systems often rely on multiple disconnected tools, such as portals for results, Google Classroom for file sharing, and informal communication channels such as Telegram. This fragmentation complicates student access to materials and creates challenges for instructors in monitoring student performance.

The project hypothesizes that a modular, domain-driven platform integrated with an AI Academic Assistant can streamline academic processes, provide context-aware support, and improve transparency in grading and learning outcomes. Major objectives include enabling students to access courses and materials, allowing instructors to manage courses and grades efficiently, and providing AI-powered guidance to enhance learning. The system will be developed using Angular for the frontend and ASP.NET Core for the backend, initially as a modular monolith and later evolving into a microservice architecture for the AI component.

The potential impact includes reduced administrative overhead, improved academic support, faster access to course materials, and a more transparent grading process. The platform could serve as a model for modernizing university academic systems both locally and regionally.

## **1. INTRODUCTION**

### **1.1 Background**

At Addis Ababa University, students currently rely on multiple systems for academic tasks. The university portal allows access to course results, while **Google Classroom** is used to distribute lecture materials. Additionally, students and instructors often use Telegram groups to exchange files and communicate urgent updates. This scattered approach increases complexity for students and instructors, reduces transparency in academic progress, and limits the ability to provide structured, timely support.

The motivation for this project is to **consolidate these fragmented processes into a single, unified platform** that improves academic efficiency and learning outcomes. The primary users of this system are university students, instructors, and administrative staff. The project aims to deliver a new platform that integrates course management, student enrollment, grading, content management, and AI-powered academic assistance. Development is expected to take one academic term, with the university as the sponsoring organization. The solution may also benefit future projects involving academic automation or AI integration in higher education.

### **1.2 The Existing System**

Currently, academic tasks are managed using multiple disconnected methods, including the university portal for viewing results, Google Classroom for file sharing, and Telegram for additional resource distribution. While functional, these systems lack integration, centralized grading, and continuous academic support, motivating the need for a unified platform.

### **1.3 Statement of the Problem**

| Element                    | Description                                                                                                                                                                                                                                                               |
| -------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **The problem of**         | Disconnected academic systems that make it difficult for students to access materials, track grades, or receive timely academic support.                                                                                                                                  |
| **Affects**                | Students, instructors, and administrative staff who must manage multiple tools to access or deliver academic content.                                                                                                                                                     |
| **And results in**         | Increased administrative overhead, student confusion, delayed feedback, and inconsistent academic performance tracking.                                                                                                                                                   |
| **Benefits of a solution** | A unified platform that integrates course management, grading, material access, and AI-based academic support. Benefits include improved transparency, streamlined administration, better learning outcomes, and enhanced communication between students and instructors. |

### **1.4 Objective of the Project**

#### **1.4.1 General Objective**

To develop a Smart University Course & Student Management Platform that consolidates academic administration, student grading, content delivery, and AI-powered academic assistance into a single, scalable, and user-friendly system.

#### **1.4.2 Specific Objectives**

- Enable students to view courses, enroll, access materials, and track grades.
- Allow instructors to manage courses, upload materials, and record grades efficiently.
- Provide an AI Academic Assistant capable of summarizing materials and answering course-related questions.
- Implement a modular architecture that can evolve from a monolith to a microservice for the AI component.
- Enhance transparency and accessibility of academic performance for students and faculty.

### **1.5 Proposed System**

The system will be a web-based platform with Angular as the frontend and ASP_NET Core as the backend, integrating course management, enrollment tracking, grading, content distribution, and AI academic support. Students can enroll in courses, view lecture materials, and track grades, while instructors can manage courses, upload content, and record performance. The AI Academic Assistant provides context-aware guidance and document summarization.

Initially, the platform will be implemented as a modular monolith with all key modules integrated. Subsequently, the AI module will be extracted as a microservice, enabling independent scaling and deployment while demonstrating best practices in modern enterprise architecture.

## **2.0 Strategic Domain Design (DDD)**

Based on the functional objectives and business value, the platform’s major business areas are classified into Core, Supporting, and Generic domains as follows:

### **2.1 Core, Supporting, and Generic Subdomains**

| **Subdomain Type**      | **Subdomain Name**                              | **Business Value / Description**                                                                                                                                                                                                                                                         |
| ----------------------- | ----------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **CORE Domain**         | **Course Management**                           | Central to the platform, this domain handles the creation, organization, and delivery of courses. It ensures that all academic activities, including enrollment, grading, and content delivery, are structured around courses, which are the primary value generator for the university. |
| **SUPPORTING Domain 1** | **Enrollment Management**                       | Manages student course registrations, enrollment status, prerequisites, and attendance tracking. Supports the core domain by ensuring students are correctly registered and monitored throughout the academic term.                                                                      |
| **SUPPORTING Domain 2** | **Assessment & Student Grading**                | Handles creation and management of assignments, exams, and grade records. Tracks student performance and provides transparency of results. Supports instructors in evaluating learning outcomes and complements the core course management functionality.                                |
| **SUPPORTING Domain 3** | **Content Management & AI Academic Assistance** | Organizes and provides access to learning materials (PDFs, notes, resources) and includes the AI Academic Assistant, which answers student questions, summarizes content, and supports learning. Enhances the core domain by providing immediate, context-aware guidance.                |
| **GENERIC Domain**      | **User & Identity Management**                  | Responsible for user authentication, role management, and access control. This domain is generic and can leverage standard solutions like Keycloak, as it provides basic identity services without containing unique academic business logic.                                            |

3.0 Tactical Domain Design: Bounded Contexts

Bounded Context 1: User Management Context
Technical Boundary
  Has its own isolated codebase, modules, and database tables dedicated only to user identity
  Contains ONLY logic for authentication, authorization, password hashing, and role definitions.
  Does NOT contain any course logic, enrollment rules, progress tracking, or prerequisites.
  Exposes user identity through public APIs or domain events, never through direct database access.
  
Bounded Context 2: Course Management Context
Technical Boundary
  Maintains a separate codebase where all course-related aggregates exist (Course, Module, Lesson).
  Contains ONLY logic for creating, updating, structuring, and publishing courses.
  Does NOT handle user accounts, authentication, or any enrollment transactions.
  Provides course availability and metadata to other contexts through API endpoints or messages.
  
Bounded Context 2: Enrollment Context
Technical Boundary
  Entirely separate module responsible for enrollment rules and policies.
  Includes ONLY logic for registering students, tracking their progress, validating prerequisites, and processing drop/add requests.
  Does NOT access the user or course database directly — relies on User Context and Course Context via APIs to fetch needed info.
  Stores its own enrollment records, progress, and statuses independently.


3.1 Bounded Context Definitions 
User Management Context - A context dedicated to identity, authentication, and role management.

Course Management Context - A context focused on course creation, structuring, content definition, and instructor assignment.

Enrollment Context - A context responsible for handling enrollment actions, prerequisite validation, and tracking student progress.

## 3.2 Context Responsibilities

| **Bounded Context Name**       | **Exclusive Responsibility** |
|-------------------------------|------------------------------|
| **User Management Context**   | This context is ONLY responsible for managing user identity, roles (Student, Instructor, Admin), and authentication credentials. |
| **Course Management Context** | This context is ONLY responsible for the lifecycle of course aggregates: creation, syllabus structure, and faculty assignment. |
| **Enrollment Context**        | This context is ONLY responsible for transactional enrollment actions: recording student registration, drop/add requests, and validating prerequisites. |

