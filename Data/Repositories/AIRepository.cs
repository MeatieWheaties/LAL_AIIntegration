using LunchAndLearn_AIIntegration.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LunchAndLearn_AIIntegration.Data.Repositories
{
    public class AIRepository
    {
        IDbContextFactory<DB> _db;

        public AIRepository(IDbContextFactory<DB> db)
        {
            _db = db;
        }

        public async Task<AIConfiguration> GetConfigAsync()
        {
            try
            {
                var _ = await _db.CreateDbContextAsync();
                var _config = _.AI.FirstOrDefault() ?? new AIConfiguration();

                _config.Context = _.AIContext.ToList();
                return _config;
            }
            catch (Exception)
            {
                return new AIConfiguration();
            }
        }

        public async Task UpdateConfiguration(AIConfiguration _config)
        {
            try
            {
                var _ = await _db.CreateDbContextAsync();

                var _existing = _.AI.FirstOrDefault();
                if(_existing == null)
                {
                    _existing = _config;
                    await _.AI.AddAsync(_existing);
                    foreach(var item in _existing.Context)
                    {
                        _.AIContext.Add(item);
                    }
                }
                else
                {
                    _existing.Objective = _config.Objective;

                    _.Update(_existing);

                    var _currentContext = _.AIContext.ToList();
                    foreach(var item in _currentContext.Where(x => !_existing.Context.Contains(x)))
                    {
                        _.AIContext.Remove(item);
                    }

                    foreach(var item in _config.Context.Where(x => !_currentContext.Contains(x)))
                    {
                        _.AIContext.Add(item);
                    }
                }

                await _.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }

        public async Task<Assessment> GetAssessmentForKobold(int id)
        {
            try
            {
                using var _ = await _db.CreateDbContextAsync();

                var _assessment = _.Assessments.FirstOrDefault(x => x.KoboldId == id);

                if (_assessment == null) return null;

                _assessment.Items = _.Items.Where(x => x.AssessmentId == _assessment.Id).ToList();

                return _assessment; 
            }
            catch (Exception) { return null; }
        }

        public async Task<Dictionary<int, Assessment>> GetAssessmentsAsync()
        {
            try
            {
                using var _ = await _db.CreateDbContextAsync();

                var _result = new Dictionary<int, Assessment>();

                var _assessments = _.Assessments.ToList();

                foreach(var _assessment in _assessments)
                {
                    _assessment.Items = _.Items.Where(x => x.AssessmentId == _assessment.Id).ToList();
                    _result.Add(_assessment.Id, _assessment);   
                }

                return _result;
            }
            catch (Exception)
            {
                return new Dictionary<int, Assessment>();
            }
        }

        public async Task CommitAssessment(Assessment assessment)
        {
            try
            {
                using var _ = await _db.CreateDbContextAsync();

                var _kobold = _.Kobolds.FirstOrDefault(x => x.Id == assessment.KoboldId);
                if (_kobold == null) return;

                var _existingAssessment = _.Assessments.FirstOrDefault(x => x.KoboldId == assessment.KoboldId);

                if(_existingAssessment == null)
                {
                    _.Assessments.Add(assessment);
                    await _.SaveChangesAsync();

                    foreach (var item in assessment.Items)
                    {
                        item.AssessmentId = assessment.Id;
                        _.Items.Add(item);
                    }
                } else
                {
                    _existingAssessment.Response = assessment.Response;
                    _existingAssessment.ConfidenceLevel = assessment.ConfidenceLevel;
                    _.Items.RemoveRange(_.Items.Where(x => x.AssessmentId == _existingAssessment.Id));
                    foreach(var item in assessment.Items)
                    {
                        item.AssessmentId = _existingAssessment.Id;
                        _.Items.Add(item);
                    }
                }

                await _.SaveChangesAsync();

            } catch (Exception)
            {

            }
        }
    }
}
