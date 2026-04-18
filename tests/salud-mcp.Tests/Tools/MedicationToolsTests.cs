using SaludMcp.Models.Cima;

namespace SaludMcp.Tests.Tools;

public class MedicationToolsTests
{
    [Fact]
    public void Medication_DefaultCollections_AreEmpty()
    {
        var med = new Medication();

        Assert.Empty(med.Documents);
        Assert.Empty(med.AtcCodes);
        Assert.Empty(med.ActiveIngredients);
        Assert.Empty(med.AdministrationRoutes);
        Assert.Empty(med.Presentations);
    }

    [Fact]
    public void Presentation_DefaultFlags_AreFalse()
    {
        var presentation = new Presentation();

        Assert.False(presentation.IsCommercial);
        Assert.False(presentation.HasSupplyProblems);
    }

    [Fact]
    public void SupplyProblem_DefaultValues_AreCorrect()
    {
        var problem = new SupplyProblem();

        Assert.False(problem.IsActive);
        Assert.Null(problem.NationalCode);
        Assert.Null(problem.Observations);
    }
}
