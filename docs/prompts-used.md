# Prompts Used for POS Project Implementation

This document records AI prompts used during the final implementation phase of the POS System project.

## 1. Project inspection prompt

**Prompt:**

Inspect the current ASP.NET Core MVC POS project and tell me what is already implemented for:
1. completed transaction saving to MongoDB,
2. cash payment,
3. card payment,
4. receipt generation,
5. report generation,
6. refund tracking,
7. discount tracking,
8. age-restricted item approval.

Do not change code yet. Give me a file-by-file summary of what exists, what is incomplete, and the safest next implementation step for MVP final submission.

**Purpose:**

Used to inspect the current project state before making additional implementation changes.

---

## 2. Report discount totals prompt

**Prompt:**

Implement the safest MVP fix for reports.

In Controllers/ReportsController.cs, when building the daily report, calculate TotalDiscounts from the selected day's transactions using the transaction discount field/discountAmount field that already exists in Models/Transactions.cs.

Also calculate the number of discounts applied if the DailyReportViewModel has a matching property. Preserve the current report page structure. Do not redesign the report UI. Do not modify appsettings.json. Keep the change small and explain the exact files changed.

**Purpose:**

Used to update the daily report so it displays discount totals and discount count using existing transaction data.

**Result:**

Implemented report discount totals and discount count.

**Commit:**

Calculate discount totals in daily reports
