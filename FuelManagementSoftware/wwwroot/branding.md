 

## PRODUCT IDENTITY

**Product Name**
PetroChain™
Blockchain-Enabled Automated Fuel Management System

**Brand Personality**

* Trustworthy
* Industrial-grade
* Government-ready
* Technologically advanced but restrained
* Designed for high-stakes infrastructure (fuel, payments, compliance)

**Core Design Principle**

> “Critical infrastructure software must feel calm, precise, and unbreakable.”

No playful elements. No trendy gradients. No consumer-app aesthetics.

---

## COLOR SCHEME (PRIMARY)

**Overall Tone**
Muted, industrial, low-glare, high-legibility. Inspired by energy, asphalt, steel, and control rooms.

### Primary Colors

* **Midnight Charcoal** `#0F172A`
  (Primary background, navigation, headers)
* **Graphite Slate** `#1E293B`
  (Secondary panels, sidebars)
* **Petro Blue** `#1D4ED8`
  (Primary action, links, blockchain confirmations)
* **Fuel Amber** `#F59E0B`
  (Warnings, low fuel indicators, tanker offloading)

### Neutral System

* **Control White** `#F8FAFC`
  (Main content background)
* **Soft Ash** `#E5E7EB`
  (Borders, dividers)
* **Muted Steel** `#64748B`
  (Secondary text, metadata)

### Status Colors

* **Success Green** `#16A34A` (Fuel available, transaction confirmed)
* **Error Red** `#DC2626` (Transaction failed, fraud flag)
* **Info Cyan** `#0891B2` (Routing updates, system notices)

---

## TYPOGRAPHY

**Primary Font (UI & Body)**
**Inter**

* Weight usage:

  * 400 (Body)
  * 500 (Labels)
  * 600 (Section headers)
  * 700 (Critical numbers, KPIs)

**Secondary Font (Numbers, Tables, Cards)**
**IBM Plex Mono**

* Used for:

  * Blockchain transaction hashes
  * Fuel volumes
  * Balances
  * Audit logs
  * Card IDs

**Font Philosophy**
Human-readable UI + machine-readable precision.

---

## LAYOUT STRUCTURE

### Global Layout

* **Left vertical sidebar** (fixed)
* **Top command bar** (contextual actions)
* **Central canvas** (cards, dashboards)
* **Right slide-over panels** (details, audits, transactions)

### Sidebar

* Dark background (`#0F172A`)
* Icons + text
* Sections:

  * Dashboard
  * Stations
  * Fuel Availability
  * Smart Cards
  * Transactions (Blockchain)
  * Routing Map
  * Fraud & Alerts
  * Reports
  * Admin / Regulators

---

## CARDS & CONTAINERS

### Cards

* Background: `#FFFFFF`
* Border radius: **10px** (never more)
* Border: `1px solid #E5E7EB`
* Shadow:
  `0 1px 2px rgba(0,0,0,0.05)`
  (Very subtle; no floating feel)

### Card Types

* KPI Cards (Fuel volume, queues, stations online)
* Station Status Cards
* Transaction Cards (immutable ledger)
* Smart Card Balance Cards

### Card Behavior

* No hover lift
* Hover = border color change only
* Click opens right-side detail drawer

---

## CORNERS & GEOMETRY

* Global radius: **8–10px**
* Buttons: **8px**
* Inputs: **6px**
* Modals: **12px**

No pills. No extreme rounding.
This is infrastructure software, not social media.

---

## BUTTON SYSTEM

### Primary Button

* Background: `#1D4ED8`
* Text: White
* Radius: 8px
* Height: 40–44px
* Label: Sentence case

### Secondary Button

* Background: `#F8FAFC`
* Border: `1px solid #CBD5E1`
* Text: `#1E293B`

### Destructive Button

* Background: `#DC2626`
* Used only for card revocation, fraud locks

---

## DATA & BLOCKCHAIN VISUALIZATION

### Blockchain Transactions

* Displayed as:

  * Sequential blocks
  * Hash preview (first 6 + last 6 chars)
  * Timestamp
  * Station ID
  * Pump ID
  * Litres dispensed
* Monospaced font
* Immutable badge icon

### Audit Logs

* Timeline style
* Left vertical line
* Each event stamped and signed

---

## MAP & ROUTING UI

### Map Theme

* Dark mode map
* Roads: muted gray
* Stations:

  * Green = Fuel available
  * Amber = Low stock
  * Red = Closed / Offloading

### Routing Panel

* Estimated wait time
* Distance
* Fuel probability score
* Recommended station highlighted with blue ring

---

## FORMS & INPUTS

* Flat inputs
* Light background
* Strong focus outline (`#1D4ED8`)
* Inline validation
* No floating labels (labels always visible)

---

## ICONOGRAPHY

* Style: Outline icons only
* Library: Lucide / Phosphor
* No filled icons
* Consistent stroke width

---

## MOTION & FEEDBACK

* Minimal animation
* 150–200ms transitions
* Only for:

  * Drawer opening
  * Status change
* No bounce, no spring

---

## DARK MODE

This system is **dark-first**, light mode is secondary.
Dark mode is default for:

* Station operators
* Control rooms
* Regulators

---

## BRANDING SUMMARY (ONE LINE)

> A secure, industrial-grade digital fuel infrastructure platform built for automation, transparency, and national-scale reliability.

---
  

---
 

**Context**
This application is built with **ASP.NET MVC** and uses **Bootstrap (classes must remain unchanged)**.
You are NOT allowed to remove, rename, or replace any Bootstrap classes (`.btn`, `.card`, `.row`, `.col-*`, `.form-control`, `.table`, etc.).

Your task is to **override Bootstrap purely at the CSS level** to achieve a **custom enterprise ERP design system**, while preserving full Bootstrap compatibility and layout behavior.

---

### RULES (NON-NEGOTIABLE)

1. **DO NOT change HTML markup**

   * Keep all Bootstrap classes exactly as they are.
   * No utility class removal.
   * No refactoring to Tailwind or custom class systems.

2. **ONLY override via CSS**

   * Use:

     * `:root` CSS variables
     * Class overrides (e.g. `.btn`, `.card`, `.table`)
     * Scoped overrides where necessary
   * Bootstrap grid, spacing, and responsiveness must remain intact.

3. **Bootstrap functionality must continue to work**

   * Modals, dropdowns, collapse, tooltips, forms, tables, alerts.
   * No JavaScript overrides unless strictly necessary.

4. **Visual identity must fully ignore default Bootstrap aesthetics**

   * No default Bootstrap blues
   * No rounded-pill buttons
   * No excessive shadows
   * No consumer-app look

---

### DESIGN GOAL

Transform Bootstrap into a **government-grade, industrial ERP UI** suitable for:

* Fuel infrastructure
* Financial transactions
* Blockchain audit trails
* Real-time operational dashboards

The result should **not look like Bootstrap**, but must **behave like Bootstrap**.

---

### GLOBAL THEME OVERRIDES

**Set CSS Variables**

* Override Bootstrap color variables:

  * `--bs-primary`
  * `--bs-secondary`
  * `--bs-success`
  * `--bs-danger`
  * `--bs-warning`
  * `--bs-info`
  * `--bs-body-bg`
  * `--bs-body-color`
  * `--bs-border-color`

Use a **dark-industrial palette**:

* Charcoal backgrounds
* Muted slate panels
* Controlled blue accents
* Amber warnings
* High-contrast readable text

---

### COMPONENT OVERRIDES (MANDATORY)

Override the following **without changing class names**:

#### Buttons (`.btn`, `.btn-primary`, `.btn-secondary`)

* Flat design
* 8px border radius
* No gradients
* Clear hierarchy
* Calm enterprise color usage

#### Cards (`.card`)

* White or dark-panel background
* Thin border
* Subtle shadow only
* No hover lift
* Used as data containers, not decorative elements

#### Forms (`.form-control`, `.form-select`)

* Flat inputs
* Always-visible labels
* Strong focus ring
* No floating labels
* No rounded pills

#### Tables (`.table`)

* Dense, readable
* Clear row separation
* Sticky headers where applicable
* Monospaced numeric columns for IDs and balances

#### Alerts (`.alert`)

* Calm, muted colors
* No bright Bootstrap defaults
* Used sparingly

#### Modals (`.modal`)

* Solid, grounded appearance
* Slightly rounded (10–12px)
* Strong hierarchy between header, body, footer

---

### TYPOGRAPHY

* Override Bootstrap font stack globally
* Use:

  * **Inter** for UI text
  * **IBM Plex Mono** for numeric data, hashes, IDs
* Increase readability:

  * Larger base font
  * Clear line height
  * Strong contrast

---

### SPACING & RHYTHM

* Respect Bootstrap spacing utilities (`.p-*`, `.m-*`)
* Internally normalize:

  * Card padding
  * Form spacing
  * Table density
* No cramped UI
* No oversized whitespace

---

### DARK MODE SUPPORT

* Implement dark mode by overriding Bootstrap variables
* Use `[data-theme="dark"]` or `.theme-dark`
* Do NOT duplicate markup
* Dark mode is default for control-room views

---

### OUTPUT EXPECTATION

Produce:

1. A **single override CSS file**
2. Compatible with Bootstrap 5+
3. No HTML changes required
4. Drop-in replacement that instantly rebrands the UI
5. Safe for ASP.NET MVC layouts and partial views

---

### FINAL CHECK

Before finishing, verify:

* Bootstrap grid still works
* Forms still validate
* Modals still open
* Dropdowns still function
* Tables still respond

If any Bootstrap behavior breaks, the solution is invalid.

 