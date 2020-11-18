import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import { TotalClientsService } from '../total-clients.service';
import {MatTable} from '@angular/material/table';
import {Sort} from '@angular/material/sort';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.sass'],
  providers: [TotalClientsService]
})
export class ListComponent implements OnInit {

  @Input()
  set data(value: { name: string, value: number }[]) {
    if (typeof value !== 'undefined' && value != null && value.length > 0) {
      this.source = value;
    } else {
      this.source = [];
    }
  }
  source: { name: string, value: number }[] = [];
  @Input() title: string;
  @Output() listNameChanged = new EventEmitter<string>();
  @Input() isSearchName: boolean;
  @ViewChild('table', { static: false }) table: MatTable<[{ name: string, value: number }]>;
  public searchName: string;
  displayedColumns: string[] = ['name','value'];
  constructor(private serv: TotalClientsService) {
  }

  ngOnInit() {

  }


  applyFilter($event: any) {
    this.searchName = $event.target.value;
    this.listNameChanged.emit(this.searchName);
  }

  private compare(a: number | string, b: number | string, isAsc: boolean) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  sortData(sort: Sort) {
    const data = this.source.slice();
    if (!sort.active || sort.direction === '') {
      this.source = data;
      return;
    }

    this.source = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'value': return this.compare(a.value, b.value, isAsc);
        case 'name': return this.compare(a.name, b.name, isAsc);
        default: return 0;
      }
    });
  }

}
