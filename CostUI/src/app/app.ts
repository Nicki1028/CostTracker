import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CostService } from './services/cost.service';
import { CostItem } from './models/cost-item';
import { ChartData } from './models/chart-data';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {

  currentPage: 'create' | 'query' = 'create';

  records: CostItem[] = [];
  chartData: ChartData[] = [];

  newCost: CostItem = {
    datetime: '2026-06-30',
    incomexpense: '支出',
    item: '',
    detail: '',
    paymethod: '',
    money: 0
  };

  query = {
    startDate: '2026-06-01',
    endDate: '2026-06-30',
    incomexpense: '全部',
    item: '全部',
    paymethod: '全部'
  };

  chartType: 'pie' | 'bar' | 'line' = 'pie';

  /** 分組依據固定用「依類別」分組，不開放使用者選擇 */
  private groupBy: 'item' | 'paymethod' | 'incomexpense' = 'item';

  private palette = ['#3b82f6', '#22c55e', '#f59e0b', '#a855f7', '#ef4444', '#06b6d4', '#ec4899', '#84cc16'];

  constructor(private costService: CostService) {}

  get chartTotal(): number {
    return this.chartData.reduce((sum, d) => sum + Math.abs(d.value), 0);
  }

  get donutBackground(): string {
    const total = this.chartTotal;
    if (total === 0) {
      return '#e5e7eb';
    }

    let acc = 0;
    const stops = this.chartData.map((d, i) => {
      const pct = (Math.abs(d.value) / total) * 100;
      const start = acc;
      acc += pct;
      return `${this.sliceColor(i)} ${start}% ${acc}%`;
    });

    return `conic-gradient(${stops.join(', ')})`;
  }

  sliceColor(i: number): string {
    return this.palette[i % this.palette.length];
  }

  slicePercent(value: number): number {
    return this.chartTotal > 0 ? (Math.abs(value) / this.chartTotal) * 100 : 0;
  }

  get chartMax(): number {
    const max = this.chartData.reduce((m, d) => Math.max(m, Math.abs(d.value)), 0);
    return max || 1;
  }

  barHeightPercent(value: number): number {
    return (Math.abs(value) / this.chartMax) * 100;
  }

  get linePoints(): string {
    const len = this.chartData.length;
    if (len === 0) {
      return '';
    }
    return this.chartData
      .map((d, i) => `${this.pointX(i, len)},${this.pointY(d.value)}`)
      .join(' ');
  }

  pointX(i: number, len: number): number {
    return len === 1 ? 50 : 4 + (i / (len - 1)) * 92;
  }

  pointY(value: number): number {
    return 95 - (Math.abs(value) / this.chartMax) * 85;
  }

  addRecord(): void {
    this.costService.addCost(this.newCost).subscribe({
      next: () => {
        alert('新增成功');
        this.resetForm();
      },
      error: err => {
        console.error(err);
        alert('新增失敗');
      }
    });
  }

  searchRecords(): void {
    this.costService.getCosts(
      this.query.startDate,
      this.query.endDate,
      this.query.incomexpense,
      this.query.item,
      this.query.paymethod
    ).subscribe({
      next: data => {
        this.records = data;
      },
      error: err => {
        console.error(err);
        alert('查詢失敗');
      }
    });
  }

  generateChart(): void {
    if (this.records.length === 0) {
      alert('請先查詢資料再產生圖表');
      return;
    }

    const grouped = new Map<string, number>();

    for (const r of this.records) {
      const key = this.groupBy === 'item' ? r.item
                : this.groupBy === 'paymethod' ? r.paymethod
                : r.incomexpense;

      grouped.set(key, (grouped.get(key) || 0) + Math.abs(r.money));
    }

    this.chartData = Array.from(grouped.entries())
      .map(([name, value]) => ({ name, value }))
      .sort((a, b) => b.value - a.value);
  }

  resetForm(): void {
    this.newCost = {
      datetime: '2026-06-30',
      incomexpense: '支出',
      item: '',
      detail: '',
      paymethod: '',
      money: 0
    };
  }

  resetQuery(): void {
    this.query = {
      startDate: '2026-06-01',
      endDate: '2026-06-30',
      incomexpense: '全部',
      item: '全部',
      paymethod: '全部'
    };

    this.records = [];
    this.chartData = [];
  }
}
