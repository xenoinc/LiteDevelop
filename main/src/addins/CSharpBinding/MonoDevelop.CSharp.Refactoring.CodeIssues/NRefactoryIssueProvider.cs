// 
// NRefactoryIssueWrapper.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp;
using MonoDevelop.Ide.Gui;
using System.Threading;
using MonoDevelop.CodeIssues;
using MonoDevelop.CSharp.Refactoring.CodeActions;
using MonoDevelop.Core;

namespace MonoDevelop.CSharp.Refactoring.CodeIssues
{
	class NRefactoryIssueProvider : CodeIssueProvider
	{
		ICSharpCode.NRefactory.CSharp.Refactoring.ICodeIssueProvider issueProvider;
		readonly string providerIdString;

		public override string IdString {
			get {
				return "refactoring.codeissues." + MimeType + "." + issueProvider.GetType ().FullName;
			}
		}

		public NRefactoryIssueProvider (ICSharpCode.NRefactory.CSharp.Refactoring.ICodeIssueProvider issue, IssueDescriptionAttribute attr)
		{
			issueProvider = issue;
			providerIdString = issueProvider.GetType ().FullName;
			Category = GettextCatalog.GetString (attr.Category ?? "");
			Title = GettextCatalog.GetString (attr.Title ?? "");
			Description = GettextCatalog.GetString (attr.Description ?? "");
			DefaultSeverity = attr.Severity;
			IssueMarker = attr.IssueMarker;
			MimeType = "text/x-csharp";
		}

		public override IEnumerable<CodeIssue> GetIssues (object ctx, CancellationToken cancellationToken)
		{
			var context = ctx as MDRefactoringContext;
			if (context == null || context.IsInvalid || context.RootNode == null)
				yield break;
				
			// Holds all the actions in a particular sibling group.
			var actionGroups = new Dictionary<object, IList<ICSharpCode.NRefactory.CSharp.Refactoring.CodeAction>> ();
			foreach (var action in issueProvider.GetIssues (context)) {
				if (cancellationToken.IsCancellationRequested)
					yield break;
				if (action.Actions == null) {
					LoggingService.LogError ("NRefactory actions == null in :" + Title);
					continue;
				}
				var actions = new List<NRefactoryCodeAction> ();
				foreach (var act in action.Actions) {
					if (cancellationToken.IsCancellationRequested)
						yield break;
					if (act == null) {
						LoggingService.LogError ("NRefactory issue action was null in :" + Title);
						continue;
					}
					var nrefactoryCodeAction = new NRefactoryCodeAction (IdString, act.Description, act, act.SiblingKey);
					if (act.SiblingKey != null) {
						// make sure the action has a list of its siblings
						IList<ICSharpCode.NRefactory.CSharp.Refactoring.CodeAction> siblingGroup;
						if (!actionGroups.TryGetValue(act.SiblingKey, out siblingGroup)) {
							siblingGroup = new List<ICSharpCode.NRefactory.CSharp.Refactoring.CodeAction> ();
							actionGroups.Add (act.SiblingKey, siblingGroup);
						}
						siblingGroup.Add (act);
						nrefactoryCodeAction.SiblingActions = siblingGroup;
					}
					actions.Add (nrefactoryCodeAction);
				}
				var issue = new CodeIssue (
					GettextCatalog.GetString (action.Description ?? ""),
					context.TextEditor.FileName,
					action.Start,
					action.End,
					IdString,
					actions
				);
				yield return issue;
			}
		}
	}
}
