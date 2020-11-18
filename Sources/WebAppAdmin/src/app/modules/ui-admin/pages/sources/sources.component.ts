import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {SourcesService} from './sources.service';
import {DownloadService} from '../../../core/services/download.service';
import {MatCheckboxChange} from '@angular/material/checkbox';
import {Sort} from '@angular/material/sort';
import {MatTable} from '@angular/material/table';
import {LoadingService} from '../../../core/services/loading.service';
import {finalize} from 'rxjs/operators';
import {FileModel, SourceDataModel} from './source-data.model';
import {PaginationParams} from '../../../ui-common/components/custom-pagination/custom-pagination.component';
import {Period} from '../../../core/models/period';
import {FileState} from './file-state.enum';
import {FileStatus} from './file-status.enum';
import {Subscription} from 'rxjs';
import {ActivatedRoute, Router} from '@angular/router';
import {AppPeriodHelper} from '../../../core/utils/app-period-helper';
import {QueryParamHelper} from '../../../core/utils/query-param-helper';

@Component({
  selector: 'app-sources',
  templateUrl: './sources.component.html',
  styleUrls: ['./sources.component.sass'],
  providers: [
    SourcesService,
    DownloadService
  ]
})
export class SourcesComponent implements OnInit, OnDestroy {

  fileStatusEnum = FileStatus;

  data: FileModel[] = [];
  displayedColumns: string[] = ['position', 'fileName', 'dateOfFile', 'dateOfTheProcessing', 'isSent', 'fileState', 'fileStatus', 'tools'];
  @ViewChild('table', {static: false, read: MatTable}) table: MatTable<FileModel[]>;
  pageIndex = 0;
  displayPageIndex = 0;
  dataLength = 0;
  pageLength = 20;
  isLoading = false;
  private sortBy = 'dateOfFile;desc';
  private searchName = '';
  period: Period = AppPeriodHelper.getDefault();
  isActiveDateCreatedFilter = true;
  subscriptions: Subscription[] = [];

  constructor(private sourcesService: SourcesService,
              private loading: LoadingService,
              private downloadService: DownloadService,
              private route: ActivatedRoute,
              private router: Router) {
  }

  ngOnInit() {
    const sub = this.route.queryParams.subscribe(queryParams => {
      if (QueryParamHelper.isContainsPeriod(queryParams)) {
        this.period = QueryParamHelper.getPeriod(queryParams);
      }
      this.refreshData();
    });
    this.subscriptions.push(sub);

    this.loading.isLoading.subscribe(val => this.isLoading = val);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  refreshData() {
    let period: Period = null;
    if (this.isActiveDateCreatedFilter) {
      period = this.period;
    }

    this.loading.setLoading(true);
    this.sourcesService.getAllFiles(this.pageIndex, this.pageLength, this.sortBy, this.searchName, period).pipe(
      finalize(() => {
        this.loading.setLoading(false);
      })
    ).subscribe(data => {
      if (data && data.dataLength > 0) {
        const source = data as SourceDataModel;
        this.dataLength = source.dataLength;
        this.data = source.data as FileModel[];
        this.displayPageIndex = this.pageIndex;
        this.table.renderRows();
      } else {
        this.data = [];
        this.dataLength = 0;
        this.pageIndex = 0;
      }
    });
  }

  applyFilter(event: Event) {
    this.searchName = (event.target as HTMLInputElement).value.trim().toLowerCase();
    this.pageIndex = 0;
    this.refreshData();
  }

  onChangePage($event: PaginationParams) {
    this.pageLength = $event.pageLength;
    this.pageIndex = $event.pageIndex;
    this.refreshData();
  }

  onSortChange($event: Sort) {
    if ($event.direction === '') {
      this.sortBy = null;
    } else {
      this.sortBy = `${$event.active};${$event.direction}`;
    }
    this.refreshData();
  }

  download(file: FileModel): void {
    this.downloadService.download(`sources/DownloadSourceFile/${file.id}`);
  }

  onChangeIsActiveDateCreatedFilter($event: MatCheckboxChange) {
    this.isActiveDateCreatedFilter = $event.checked;
    this.pageIndex = 0;
    this.refreshData();
  }

  resolveIsSentToApi(file: FileModel): string {
    if (!file.isForApi) {
      return '---';
    } else if (file.isSent) {
      return 'Sent';
    } else {
      return 'Waiting';
    }
  }

  resolveFileStateName(fileState: FileState): string {
    switch (fileState) {
      case FileState.REGISTERED:
        return 'Register';
      case FileState.LOADED:
        return 'Load';
      case FileState.IMPORTED:
        return 'Import';
      default:
        return '';
    }
  }

  resolveFileStatusName(file: FileModel): string {
    switch (file.fileStatus) {
      case FileStatus.NONE:
      case FileStatus.SUCCESS:
        return 'Success';
      case FileStatus.FAILED:
        return 'Failed';
      default:
        return '';
    }
  }

  selectedDateRangeChanged($event: Period) {
    this.pageIndex = 0;
    this.period = $event;
    this.refreshData();
  }

  goBack() {
    this.router.navigate(['/analytics']);
  }
}
