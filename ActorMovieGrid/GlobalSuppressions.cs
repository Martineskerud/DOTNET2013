// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.


//From bindablebase.cs: suppressed because nobody really knew how to fix this. 
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Scope = "member", Target = "ActorMovieGrid.Common.BindableBase.#SetProperty`1(!!0&,!!0)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Scope = "member", Target = "ActorMovieGrid.Common.BindableBase.#SetProperty`1(!!0&,!!0,System.String)")]


//Suppressed, changing the casing of martinbe to Martinbe is difficult and would only cause more problems as well as it still not being a valid word. 
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "martinbe", Scope = "type", Target = "ActorMovieGrid.ActorMovieServiceReference.martinbeEntities")]

//Cant fix the spelling of this, would cause more errors and cause issues with database connection. 
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "martinbe", Scope = "type", Target = "ActorMovieGrid.ActorMovieServiceReference.martinbeEntities")]