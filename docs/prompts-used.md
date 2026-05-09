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

## 3. Role/login inspection prompt

**Prompt:**

Now inspect the current employee authentication flow in detail.

**Purpose:**

Used to understand how employee login worked before adding role-based access.

**Result:**

Identified where login session data was being set and where manager/employee role logic should be added.

## 4. Role-based access prompt

**Prompt:**

Implement the safest first step for role-based access without breaking the current login flow.

**Purpose:**

Used to add manager/employee role handling while preserving the existing login process.

**Result:**

Added session-based role handling so the system could distinguish managers from employees.

## 5. Manager PID mapping fix prompt

**Prompt:**

Apply the smallest safe fix only. Update DetermineRole() in Controllers/AuthController.cs so it matches the real employee PID schema:
- if PID starts with "M" -> return "Manager"
- otherwise return "Employee".

**Purpose:**

Used to fix manager detection because the real employee PID values were M1, M2, and E1.

**Result:**

Manager logins were correctly recognized when the PID started with M.

## 6. Manager-only product voiding prompt

**Prompt:**

Now implement the actual manager-only product voiding feature using the reusable session-based manager guard.

**Purpose:**

Used to add a manager-only product void function.

**Result:**

Managers can void products, employees cannot, and voided products are marked in MongoDB instead of being deleted.

## 7. Block sale of voided products prompt

**Prompt:**

Now implement the next incremental step: block the sale of voided products in the current sale flow.

**Purpose:**

Used to make sure voided products could not still be sold.

**Result:**

The transaction flow now blocks products where isVoided is true.

## 8. Lookup void status display prompt

**Prompt:**

The item is not visually showing as voided after I press Void Product, because the lookup/details UI still says "In Stock". Apply the smallest safe fix.

**Purpose:**

Used to fix the lookup page so voided products display the correct status.

**Result:**

The lookup page now prioritizes Voided status before Out of Stock or In Stock.

## 9. Printable receipt prompt

**Prompt:**

Add the smallest safe print receipt feature.

In Views/Transactions/SaleComplete.cshtml, add a visible "Print Receipt" button that calls window.print(). Add minimal print-only CSS so the receipt content prints cleanly and navigation/buttons do not print if practical.

Do not add PDF generation, email sending, printer hardware logic, or new services. Treat the existing SaleComplete page as the receipt page. Do not modify appsettings.json. Keep the change small and explain the exact files changed.

**Purpose:**

Used to add a printable receipt option after a completed sale.

**Result:**

Added a Print Receipt button to the sale completion page. The button calls `window.print()`, and print-specific CSS hides navigation/action controls so the receipt prints more cleanly.

## 10. Cash tender and change due prompt

**Prompt:**

Inspect the current cash payment flow in TransactionsController and Views/Transactions/MakeSale.cshtml.

If cash received and change due are not implemented, add the smallest MVP version:
- show a Cash Received input only when Cash is selected,
- validate cash received is greater than or equal to the transaction total,
- calculate Change Due,
- pass cash received and change due to the SaleComplete page if the current model supports it, or add minimal view model properties if needed.

Do not redesign the transaction flow. Do not modify appsettings.json. Keep Card/Mobile Pay behavior unchanged. Keep changes small and list the files changed.

**Purpose:**

Used to improve the cash payment flow by recording cash received, validating sufficient payment, and calculating change due.

Added cash received and change due support for cash payments. The sale screen now shows a Cash Received field when Cash is selected, validates that the cash received is enough to cover the total, calculates change due, saves cash values on the transaction, and displays cash received/change due on the sale completion page.

## 11. Manager approval for age-restricted sales prompt

**Prompt:**

Update the age-restricted item approval flow to require manager credentials instead of any employee credentials.

Use the existing EmployeeServices.ValidateManagerCredentialsAsync method if available, matching the discount/refund manager approval style. Do not redesign the sale flow. Keep the current two-step approval UI. Do not modify appsettings.json. Keep changes small and explain the exact files changed.

**Purpose:**

Used to make age-restricted item approval consistent with other manager-only approval workflows.

**Result:**

Updated the age-restricted item approval flow so restricted item approval now requires manager credentials. The existing two-step approval UI was preserved, and the approval check now uses the manager credential validation method instead of accepting any employee credentials.

## 12. Remove Mobile Pay prompt

**Prompt:**

Remove Mobile Pay as a payment option from the POS system.

Keep only Cash and Card as valid payment methods. Update the sale screen payment dropdown, controller validation if needed, transaction model logic if needed, and any related display text so the application no longer presents Mobile Pay as an option.

Do not redesign the transaction flow. Do not modify appsettings.json. Keep existing Cash behavior with cash received/change due. Keep existing Card behavior as the simulated card payment option. Keep changes small and explain the exact files changed.

**Purpose:**

Used to align the payment options with the project requirement that only cash and card payments are supported.

**Result:**

Removed Mobile Pay from the payment options. The sale screen now only displays Cash and Card, and the transaction controller validates payment methods so only Cash or Card are accepted.

## 13. Remove UPC wording prompt

**Prompt:**

Remove UPC-related wording from the POS app and keep item lookup/search based on SKU.

Update visible UI text, labels, helper text, and documentation comments if needed so the app no longer claims UPC lookup or UPC scanning support. Keep SKU search and item name search if currently supported. Do not change database schema unless necessary. Do not modify appsettings.json. Keep changes small and explain the exact files changed.

**Purpose:**

Used to align the item lookup and sale entry wording with the project decision to use SKU only instead of UPC.

**Result:**

Removed UPC-related wording from the POS app. Item lookup and sale entry now describe SKU-based search/entry, while still keeping existing SKU and item name search behavior where already supported.

## 14. Unit test project prompt

**Prompt:**

Create an xUnit test project for this ASP.NET Core MVC POS solution.

Add unit tests for the most critical project behavior, including:
- cash payment validation,
- change due calculation,
- manager role detection,
- blocked/voided product sale rules,
- transaction model behavior,
- report totals for sales, refunds, and discounts.

Do not modify appsettings.json. Keep tests independent from live MongoDB if possible.

**Purpose:**

Used to create unit test cases for the required final submission testing section.

**Result:**

Created a separate xUnit test project with unit tests for core POS business behavior. The test suite passed with 27 tests succeeded and 0 failed.
