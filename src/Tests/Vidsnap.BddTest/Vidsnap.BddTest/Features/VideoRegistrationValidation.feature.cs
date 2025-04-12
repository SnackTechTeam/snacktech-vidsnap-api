﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by Reqnroll (https://www.reqnroll.net/).
//      Reqnroll Version:2.0.0.0
//      Reqnroll Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Vidsnap.BddTest.Features
{
    using Reqnroll;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class VideoSubmissionValidationFeature : object, Xunit.IClassFixture<VideoSubmissionValidationFeature.FixtureData>, Xunit.IAsyncLifetime
    {
        
        private global::Reqnroll.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private static global::Reqnroll.FeatureInfo featureInfo = new global::Reqnroll.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Video submission validation", null, global::Reqnroll.ProgrammingLanguage.CSharp, featureTags);
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "VideoRegistrationValidation.feature"
#line hidden
        
        public VideoSubmissionValidationFeature(VideoSubmissionValidationFeature.FixtureData fixtureData, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
        }
        
        public static async System.Threading.Tasks.Task FeatureSetupAsync()
        {
        }
        
        public static async System.Threading.Tasks.Task FeatureTearDownAsync()
        {
        }
        
        public async System.Threading.Tasks.Task TestInitializeAsync()
        {
            testRunner = global::Reqnroll.TestRunnerManager.GetTestRunnerForAssembly(featureHint: featureInfo);
            if (((testRunner.FeatureContext != null) 
                        && (testRunner.FeatureContext.FeatureInfo.Equals(featureInfo) == false)))
            {
                await testRunner.OnFeatureEndAsync();
            }
            if ((testRunner.FeatureContext == null))
            {
                await testRunner.OnFeatureStartAsync(featureInfo);
            }
        }
        
        public async System.Threading.Tasks.Task TestTearDownAsync()
        {
            await testRunner.OnScenarioEndAsync();
            global::Reqnroll.TestRunnerManager.ReleaseTestRunner(testRunner);
        }
        
        public void ScenarioInitialize(global::Reqnroll.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public async System.Threading.Tasks.Task ScenarioStartAsync()
        {
            await testRunner.OnScenarioStartAsync();
        }
        
        public async System.Threading.Tasks.Task ScenarioCleanupAsync()
        {
            await testRunner.CollectScenarioErrorsAsync();
        }
        
        async System.Threading.Tasks.Task Xunit.IAsyncLifetime.InitializeAsync()
        {
            await this.TestInitializeAsync();
        }
        
        async System.Threading.Tasks.Task Xunit.IAsyncLifetime.DisposeAsync()
        {
            await this.TestTearDownAsync();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Submitting invalid video data should return validation errors")]
        [Xunit.TraitAttribute("FeatureTitle", "Video submission validation")]
        [Xunit.TraitAttribute("Description", "Submitting invalid video data should return validation errors")]
        [Xunit.InlineDataAttribute("00000000-0000-0000-0000-000000000001", "not-an-email", "MeuVideo", "mp4", "100", "60", "O EmailUsuario deve ser um endereço de e-mail válido.", new string[0])]
        [Xunit.InlineDataAttribute("00000000-0000-0000-0000-000000000001", "user@email.com", "", "mp4", "100", "60", "O NomeVideo é obrigatório.", new string[0])]
        [Xunit.InlineDataAttribute("00000000-0000-0000-0000-000000000001", "user@email.com", "MeuVideo", "exe", "100", "60", "A Extensão deve ser um dos formatos suportados: mp4, avi, mov, mkv", new string[0])]
        [Xunit.InlineDataAttribute("00000000-0000-0000-0000-000000000001", "user@email.com", "MeuVideo", "mp4", "0", "60", "O Tamanho do vídeo deve ser maior que zero.", new string[0])]
        [Xunit.InlineDataAttribute("00000000-0000-0000-0000-000000000001", "user@email.com", "MeuVideo", "mp4", "100", "0", "A Duração do vídeo deve ser maior que zero.", new string[0])]
        public async System.Threading.Tasks.Task SubmittingInvalidVideoDataShouldReturnValidationErrors(string idUsuario, string email, string nome, string ext, string tam, string dur, string mensagemErroEsperada, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("IdUsuario", idUsuario);
            argumentsOfScenario.Add("Email", email);
            argumentsOfScenario.Add("Nome", nome);
            argumentsOfScenario.Add("Ext", ext);
            argumentsOfScenario.Add("Tam", tam);
            argumentsOfScenario.Add("Dur", dur);
            argumentsOfScenario.Add("MensagemErroEsperada", mensagemErroEsperada);
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Submitting invalid video data should return validation errors", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 3
  this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
                global::Reqnroll.Table table2 = new global::Reqnroll.Table(new string[] {
                            "IdUsuario",
                            "EmailUsuario",
                            "NomeVideo",
                            "Extensao",
                            "Tamanho",
                            "Duracao"});
                table2.AddRow(new string[] {
                            string.Format("{0}", idUsuario),
                            string.Format("{0}", email),
                            string.Format("{0}", nome),
                            string.Format("{0}", ext),
                            string.Format("{0}", tam),
                            string.Format("{0}", dur)});
#line 4
    await testRunner.GivenAsync("I have the following invalid video data:", ((string)(null)), table2, "Given ");
#line hidden
#line 7
    await testRunner.WhenAsync("I POST this data to \"/api/videos\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 8
    await testRunner.ThenAsync("the response status code should be 400", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
#line 9
    await testRunner.AndAsync(string.Format("the response should contain the error \"{0}\"", mensagemErroEsperada), ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : object, Xunit.IAsyncLifetime
        {
            
            async System.Threading.Tasks.Task Xunit.IAsyncLifetime.InitializeAsync()
            {
                await VideoSubmissionValidationFeature.FeatureSetupAsync();
            }
            
            async System.Threading.Tasks.Task Xunit.IAsyncLifetime.DisposeAsync()
            {
                await VideoSubmissionValidationFeature.FeatureTearDownAsync();
            }
        }
    }
}
#pragma warning restore
#endregion
