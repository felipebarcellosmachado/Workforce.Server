using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Workforce.Domain.Core.StaffingScheduleManagement.BaseStaffingSchedule.Entity;
using Workforce.Realization.Infrastructure.Persistence.Core.StaffingScheduleManagement.BaseStaffingSchedule.Repository;

namespace Workforce.Server.Controllers.Core.StaffingScheduleManagement.BaseStaffingSchedule
{
    [ApiController]
    [Route("api/core/staffing-schedule-management/basestaffingscheduleresource")]
    public class BaseStaffingScheduleResourceController : ControllerBase
    {
        private readonly BaseStaffingScheduleResourceRepository repository;

        public BaseStaffingScheduleResourceController(BaseStaffingScheduleResourceRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetBaseStaffingScheduleResourceById")]
        public async Task<ActionResult<BaseStaffingScheduleResource>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var entity = await repository.GetByIdAsync(id, ct);

                if (entity == null)
                {
                    return NotFound($"BaseStaffingScheduleResource com ID {id} não encontrado");
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingScheduleResource: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<BaseStaffingScheduleResource>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllAsync(ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter BaseStaffingScheduleResources: {ex.Message}");
            }
        }

        [HttpGet("all/schedule/{baseStaffingScheduleId:int}")]
        public async Task<ActionResult<IList<BaseStaffingScheduleResource>>> GetAllByBaseStaffingScheduleIdAsync(int baseStaffingScheduleId, CancellationToken ct = default)
        {
            try
            {
                var entities = await repository.GetAllByBaseStaffingScheduleIdAsync(baseStaffingScheduleId, ct);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter Resources por BaseStaffingScheduleId: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseStaffingScheduleResource>> InsertAsync([FromBody] BaseStaffingScheduleResource entity, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entity.Name))
                {
                    return BadRequest("Nome é obrigatório");
                }

                var insertedEntity = await repository.InsertAsync(entity, ct);
                return Created($"/api/core/staffing-schedule-management/basestaffingscheduleresource/{insertedEntity.Id}", insertedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao inserir BaseStaffingScheduleResource: {ex.Message}");
            }
        }

        [HttpPost("batch")]
        public async Task<ActionResult<IList<BaseStaffingScheduleResource>>> InsertBatchAsync([FromBody] IList<BaseStaffingScheduleResource> entities, CancellationToken ct = default)
        {
            try
            {
                if (entities == null || entities.Count == 0)
                {
                    return BadRequest("Lista de recursos não pode estar vazia");
                }

                var insertedEntities = await repository.InsertBatchAsync(entities, ct);
                return Ok(insertedEntities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao inserir lote de BaseStaffingScheduleResources: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult<BaseStaffingScheduleResource>> UpdateAsync([FromBody] BaseStaffingScheduleResource entity, CancellationToken ct = default)
        {
            try
            {
                var updatedEntity = await repository.UpdateAsync(entity, ct);

                if (updatedEntity == null)
                {
                    return NotFound($"BaseStaffingScheduleResource com ID {entity.Id} não encontrado");
                }

                return Ok(updatedEntity);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar BaseStaffingScheduleResource: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var result = await repository.DeleteByIdAsync(id, ct);

                if (!result)
                {
                    return NotFound($"BaseStaffingScheduleResource com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir BaseStaffingScheduleResource: {ex.Message}");
            }
        }

        [HttpDelete("all/schedule/{baseStaffingScheduleId:int}")]
        public async Task<ActionResult> DeleteAllByBaseStaffingScheduleIdAsync(int baseStaffingScheduleId, CancellationToken ct = default)
        {
            try
            {
                await repository.DeleteAllByBaseStaffingScheduleIdAsync(baseStaffingScheduleId, ct);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir Resources do BaseStaffingSchedule: {ex.Message}");
            }
        }

        /// <summary>
        /// Gera recursos em lote a partir de opções configuráveis.
        /// </summary>
        [HttpPost("generate")]
        public async Task<ActionResult<IList<BaseStaffingScheduleResource>>> GenerateAsync([FromBody] ResourceGenerationOptions options, CancellationToken ct = default)
        {
            try
            {
                if (options.Count <= 0)
                {
                    return BadRequest("Count deve ser maior que zero");
                }

                var resources = new List<BaseStaffingScheduleResource>();

                if (options.IndividualProfiles)
                {
                    for (int i = 1; i <= options.Count; i++)
                    {
                        var resource = CreateResourceFromOptions(options, $"{options.NamePrefix} #{i}");
                        resources.Add(resource);
                    }
                }
                else
                {
                    var resource = CreateResourceFromOptions(options, options.NamePrefix);
                    resource.Quantity = options.Count;
                    resource.MaxQuantity = options.Count;
                    resources.Add(resource);
                }

                var insertedResources = await repository.InsertBatchAsync(resources, ct);
                return Ok(insertedResources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao gerar Resources em lote: {ex.Message}");
            }
        }

        private static BaseStaffingScheduleResource CreateResourceFromOptions(ResourceGenerationOptions options, string name)
        {
            var resource = new BaseStaffingScheduleResource
            {
                BaseStaffingScheduleId = options.BaseStaffingScheduleId,
                Name = name,
                WorkingTimeId = options.WorkingTimeId,
                JobTitleId = options.JobTitleId,
                FunctionalUnitId = options.FunctionalUnitId,
                ClassId = options.ClassId,
                Quantity = 1,
                MinQuantity = 0,
                MaxQuantity = 1,
                CostPerHour = options.CostPerHour,
                OvertimeCostPerHour = options.OvertimeCostPerHour,
                FixedMonthlyCost = options.FixedMonthlyCost,
                AbsenteeismRate = options.AbsenteeismRate,
                Priority = options.Priority,
                IsActive = true
            };

            foreach (var skillId in options.SkillIds)
            {
                resource.Skills.Add(new BaseStaffingScheduleResourceSkill
                {
                    SkillId = skillId
                });
            }

            foreach (var qualificationId in options.QualificationIds)
            {
                resource.Qualifications.Add(new BaseStaffingScheduleResourceQualification
                {
                    QualificationId = qualificationId
                });
            }

            foreach (var behaviourId in options.BehaviourIds)
            {
                resource.Behaviours.Add(new BaseStaffingScheduleResourceBehaviour
                {
                    BehaviourId = behaviourId
                });
            }

            return resource;
        }
    }
}
