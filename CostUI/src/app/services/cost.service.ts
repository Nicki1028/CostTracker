import { Injectable } from '@angular/core';
import { HttpClient, HttpParams} from '@angular/common/http';
import { CostItem } from '../models/cost-item';
import { ChartData } from '../models/chart-data';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CostService {
  private apiUrl = 'http://localhost:5000/api/costs';

  constructor(private http: HttpClient) {}

  getCosts(
    start: string,
    end: string,
    incomexpense: string,
    item: string,
    paymethod: string
  ): Observable<CostItem[]> {
    let params = new HttpParams()
      .set('start', start)
      .set('end', end);

    if (incomexpense !== '全部') {
      params = params.set('incomexpense', incomexpense);
    }

    if (item !== '全部') {
      params = params.set('item', item);
    }

    if (paymethod !== '全部') {
      params = params.set('paymethod', paymethod);
    }

    return this.http.get<CostItem[]>(this.apiUrl, { params });
  }

  addCost(cost: CostItem): Observable<void> {
    return this.http.post<void>(this.apiUrl, cost);
  }


}
