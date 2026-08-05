using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    public class ContractCalculationService
    {
        public ContractCalculationResult Calculate(ContractCreateDto dto)
        {
            decimal financedBalance = dto.SalePrice - dto.DownPayment;
            decimal interestAmount = financedBalance * dto.LateInterestRate;
            decimal totalToPay = financedBalance + interestAmount;
            decimal baseInstallment = Math.Round(totalToPay / dto.InstallmentCount, 2);

            var result = new ContractCalculationResult
            {
                FinancedBalance = financedBalance,
                InterestAmount = interestAmount,
                TotalToPay = totalToPay,
                InstallmentAmount = baseInstallment
            };

            decimal accumulated = 0;
            for (int i = 1; i <= dto.InstallmentCount; i++)
            {
                decimal amount;
                if (i < dto.InstallmentCount)
                {
                    amount = baseInstallment;
                    accumulated += amount;
                }
                else
                {
                    amount = totalToPay - accumulated;
                }

                result.Installments.Add(new InstallmentPreview
                {
                    InstallmentNumber = i,
                    ExpectedAmount = amount,
                    DueDate = CalculateDueDate(dto.FirstDueDate, i, dto.FrequencyCode)
                });
            }

            return result;
        }

        private DateTime CalculateDueDate(DateTime firstDueDate, int installmentNumber, string frequencyCode)
        {
            // installmentNumber 1 = la fecha base tal cual, sin sumar nada
            int offset = installmentNumber - 1;

            return frequencyCode.ToUpper() switch
            {
                "SEMANAL" => firstDueDate.AddDays(7 * offset),
                "QUINCENAL" => firstDueDate.AddDays(15 * offset),
                "MENSUAL" => firstDueDate.AddMonths(offset),
                "TRIMESTRAL" => firstDueDate.AddMonths(3 * offset),
                _ => throw new ArgumentException($"Frecuencia no reconocida: {frequencyCode}")
            };
        }

    }
}
