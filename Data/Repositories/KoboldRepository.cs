using LunchAndLearn_AIIntegration.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LunchAndLearn_AIIntegration.Data.Repositories
{
    public class KoboldRepository
    {
        readonly IDbContextFactory<DB> _db;

        public KoboldRepository(IDbContextFactory<DB> db)
        {
            _db = db;
        }

        public async Task<Kobold> GetKobold(int id)
        {
            try
            {
                using var _ = await _db.CreateDbContextAsync();
                return _.Kobolds.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex) { return null; }
        }

        public async Task<Kobold> AddKobold(string koboldName, string koboldMessage)
        {
            try
            {
                using var _ = await _db.CreateDbContextAsync();

                var _kobold = new Kobold
                {
                    Name = koboldName,
                    Message = koboldMessage
                };

                _.Kobolds.Add(_kobold);
                await _.SaveChangesAsync();

                var _aiAssessment = new AI_KoboldAssessment
                {
                    KoboldId = _kobold.Id,
                    Assessment = KoboldAssessment.New
                };

                _.AI_KoboldAssessments.Add(_aiAssessment);
                await _.SaveChangesAsync();

                return _kobold;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> RemoveKobold(int id)
        {
            try
            {
                using var _ = await _db.CreateDbContextAsync();

                var _kobold = _.Kobolds.FirstOrDefault(x => x.Id == id);
                if(_kobold != null)
                {
                    _.Kobolds.Remove(_kobold);

                    var _assessment = _.Assessments.FirstOrDefault(x => x.KoboldId == id);
                    if(_assessment != null)
                    {
                        var _items = _.Items.Where(x => x.AssessmentId == _assessment.Id);
                        _.Items.RemoveRange(_items);
                        _.Assessments.Remove(_assessment);
                    }

                    await _.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
