using CommandLine.Text;
using CommandLine;

namespace TransferScenarioItems
{
    public class Options
    {
        [Option("scenario-icons", Required = true, HelpText = @"folder of the scenario icons eg. [EGS]\Content\Scenarios\Reforged Eden\SharedData\Content\Bundles\ItemIcons ")]
        public string ScenarioIconsFolder { get; set; }

        [Option("eah-icons", Required = false, HelpText = @"folder of the EAH icons e.g. [EGS]\DedicatedServer\EmpyrionAdminHelper\Items")]
        public string EAHIconsFolder { get; set; }

        [Option("target-folder", Required = true, HelpText = @"target folder of the id icons e.g. [EGS]\Content\Mods\EWALoader\EWA\ClientApp\dist\ClientApp\assets\Items")]
        public string TargetFolder { get; set; }

        [Option("name-id-mappingfile", Required = true, HelpText = @"JSON for the name id mapping e.g. from EmpyrionScripting: [EGS]\Saves\Games\DefaultRE\Mods\EmpyrionScripting\NameIdMapping.json")]
        public string NameIdMappingFile { get; set; }
    }
}