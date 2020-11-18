import {Component, ElementRef, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {TradeAccountService} from '../../../core/services/trade-account.service';
import {TradeAccountModel} from '../../../core/models/trade-account.model';
import {catchError, finalize} from 'rxjs/operators';
import {of} from 'rxjs';
import {FormHelper} from '../../../core/utils/form-helper';
import {LoadingService} from '../../../core/services/loading.service';

@Component({
  selector: 'app-trade-account-search-box',
  templateUrl: './trade-account-search-box.component.html',
  styleUrls: ['./trade-account-search-box.component.sass']
})
export class TradeAccountSearchBoxComponent implements OnInit {

  form: FormGroup;
  searchName = '';
  @Output()
  tradeAccountSearchChanged = new EventEmitter<TradeAccountModel>();
  @Output()
  clearSearchName = new EventEmitter();
  isSubmitted = false;
  isSent = false;
  modelState: any = null;
  @ViewChild('formRef', {static: false}) formRef: ElementRef;

  constructor(private fBuilder: FormBuilder,
              private tradeAccountService: TradeAccountService,
              private loading: LoadingService) {
  }

  ngOnInit() {
    this.initForm();
  }

  private initForm() {
    this.form = this.fBuilder.group({
      search: ['', [Validators.required]]
    });
    this.form.get('search').valueChanges.subscribe(value => {
      this.isSubmitted = false;
      if (value === '') {
        this.clearSearchName.emit();
      }
    });
  }

  getTradeAccount() {
    this.isSubmitted = true;

    if (this.isSent || this.form.invalid) {
      if (this.form.invalid) {
        FormHelper.focusFirstControlOnError(this.form, this.formRef);
      }
      return;
    }

    this.isSent = true;
    this.modelState = null;
    this.isSubmitted = true;
    this.loading.setLoading(true);
    this.tradeAccountService.getTradeAccountBySearchName(this.searchName.trim())
      .pipe(
        finalize(() => {
          this.isSent = false;
          this.loading.setLoading(false);
        }),
        catchError(err => {
          this.modelState = err.error.modelState;
          FormHelper.processModelState(this.form, this.modelState);
          return of(null);
        })
      )
      .subscribe(data => this.tradeAccountSearchChanged.emit(data));
  }

  isErrorState() {
    const control = this.form.get('search');
    if (this.form
      && control
      && this.isSubmitted
      && control.invalid
      && control.pristine) {
      return true;
    }
    return false;
  }
}
