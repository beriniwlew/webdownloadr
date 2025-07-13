# 0004: Replace Autofac with .NET Core Dependency Injection

Status: Accepted  
Date: 2024-01-15  
Supersedes: N/A  
Superseded-by: N/A

## Context

Initially, this repository employed Autofac for dependency injection. At the time of adoption, Autofac was preferred due to its robust feature set, including:

- Support for advanced scenarios such as decorators and modules, which could be placed close to their corresponding implementations.
- A well-established history of stability and maturity, having been used in various projects before .NET Core's built-in DI was fully featured.

As the .NET ecosystem evolved, the built-in DI container began to meet the needs of our project without introducing the added complexity associated with Autofac. The .NET DI framework has matured significantly, offering sufficient functionality for typical use cases, including:

- Improved support for configuration through extension methods.
- A simpler learning curve for new contributors familiar with .NET conventions.

## Decision

Based on community feedback and a review of our project's requirements, we have decided to remove Autofac from this template and transition to using .NET's built-in dependency injection infrastructure. This decision aligns with the goal of simplifying the codebase and reducing external dependencies.

## Consequences

**Positive outcomes:**
- **Simplified Codebase**: Removing Autofac results in a cleaner, more maintainable codebase that adheres to .NET standards.
- **Reduced Complexity**: The transition eliminates the need for additional files and configurations specific to Autofac, making the project easier to understand for new contributors.
- **Standardization**: Adopting .NET's built-in DI promotes consistency with other .NET projects, making it easier for developers familiar with the framework to contribute effectively.

**Negative outcomes:**
- Loss of some advanced Autofac features (though not needed for this project)
- Migration effort required for existing implementations

**Follow-up tasks:**
- Remove Autofac dependencies from project files
- Update service registration code to use .NET DI patterns
- Update documentation to reflect the change

## Alternatives Considered

- **Continue using Autofac** – Would maintain advanced features but add unnecessary complexity
- **Use a different DI container** – Other containers like Ninject or Unity were considered but rejected for similar reasons

## References

- [Issue #649: Why is this repo using Autofac instead of .NET's own DI infrastructure?](https://github.com/ardalis/CleanArchitecture/issues/649) - Discussion that led to this decision.
- [Getting Started with Architecture Decision Records](https://ardalis.com/getting-started-with-architecture-decision-records/) - Resource on ADR best practices.
