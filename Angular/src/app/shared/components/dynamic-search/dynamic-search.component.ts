import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faXmark, faPlus } from '@fortawesome/free-solid-svg-icons';

export enum SearchTypes {
  Text = 'text',
  Number = 'number',
  Date = 'date',
  MultiSelect = 'multiSelect',
}

export interface SearchItem {
  name: string;
  distplayName: string;
  value?: string | string[];
  operator?: SearchOperators;
  type: SearchTypes;
  selected?: boolean;
  listValues?: { value: string; displayName: string }[];
  handleAutomatically?: boolean;
  isValueSelectable?: boolean;
}

export enum SearchOperators {
  Equal = 'Equal',
  NotEqual = 'NotEqual',
  GreaterThan = 'GreaterThan',
  GreaterThanOrEqual = 'GreaterThanOrEqual',
  LessThan = 'LessThan',
  LessThanOrEqual = 'LessThanOrEqual',
  Contains = 'Contains',
  StartsWith = 'StartsWith',
  EndsWith = 'EndsWith',
  IsNull = 'IsNull',
  IsNotNull = 'IsNotNull',
  In = 'In',
  NotIn = 'NotIn',
}

@Component({
  selector: 'app-dynamic-search',
  templateUrl: './dynamic-search.component.html',
  styleUrls: ['./dynamic-search.component.css'],
  standalone: true,
  imports: [FormsModule, CommonModule, FontAwesomeModule],
})
export class DynamicSearchComponent {
  @Input({ required: true }) public searchItems: SearchItem[] = [];
  @Input() public disabled: boolean = false;
  @Output() public onItemSelected = new EventEmitter<SearchItem>();
  @Output() public onItemRemoved = new EventEmitter<SearchItem>();

  public selectedSearchItem?: SearchItem;
  public SearchTypes = SearchTypes;
  public faXmark = faXmark;
  public faPlus = faPlus;
  public textSearchOperators = [
    { name: 'Equal to', value: SearchOperators.Equal },
    { name: 'Not equal to', value: SearchOperators.NotEqual },
    { name: 'Contains', value: SearchOperators.Contains },
    { name: 'Starts with', value: SearchOperators.StartsWith },
    { name: 'Ends with', value: SearchOperators.EndsWith },
    { name: 'Is empty', value: SearchOperators.IsNull },
    { name: 'Is not empty', value: SearchOperators.IsNotNull },
  ];
  public numberSearchOperators = [
    { name: 'Equal to', value: SearchOperators.Equal },
    { name: 'Not equal to', value: SearchOperators.NotEqual },
    { name: 'Greater than', value: SearchOperators.GreaterThan },
    { name: 'Greater than or equal to', value: SearchOperators.GreaterThanOrEqual },
    { name: 'Less than', value: SearchOperators.LessThan },
    { name: 'Less than or equal to', value: SearchOperators.LessThanOrEqual },
    { name: 'Is empty', value: SearchOperators.IsNull },
    { name: 'Is not empty', value: SearchOperators.IsNotNull },
  ];
  public multiSelectListOperators = [
    { name: 'In', value: SearchOperators.In },
    { name: 'Not in', value: SearchOperators.NotIn },
  ];

  public getNotSelectedSearchItem(): SearchItem[] {
    return this.searchItems.filter((_) => !_.selected);
  }

  public getSelectedSearchItem(): SearchItem[] {
    return this.searchItems.filter((_) => _.selected);
  }

  public selectItemModelChange() {
    if (!this.selectedSearchItem) return;
    this.selectedSearchItem.operator = this.selectedSearchItem.isValueSelectable ? SearchOperators.Equal : undefined;
    this.selectedSearchItem.value = undefined;
  }

  public operatorChange() {
    if (!this.selectedSearchItem || !this.isNullOperatorSelected()) return;
    this.selectedSearchItem.value = undefined;
  }

  public selectItem() {
    if (!this.selectedSearchItem || !this.isSelectable()) return;
    this.selectedSearchItem.selected = true;
    this.selectedSearchItem = undefined;
    this.onItemSelected.emit(this.selectedSearchItem);
  }

  public getOperatorsByType(searchItem: SearchItem) {
    if (!searchItem || searchItem.isValueSelectable) return;
    switch (searchItem.type) {
      case SearchTypes.Text:
        return this.textSearchOperators;
      case SearchTypes.Date:
      case SearchTypes.Number:
        return this.numberSearchOperators;
      case SearchTypes.MultiSelect:
        return this.multiSelectListOperators;
      default:
        return;
    }
  }

  public isNullOperatorSelected() {
    return this.selectedSearchItem?.operator === SearchOperators.IsNull || this.selectedSearchItem?.operator === SearchOperators.IsNotNull;
  }

  public isSelectable() {
    if (!this.selectedSearchItem || !this.selectedSearchItem.operator) return false;
    if (this.isNullOperatorSelected()) return true;
    if (!this.selectedSearchItem.value) return false;
    if (this.selectedSearchItem.type === SearchTypes.Text && this.selectedSearchItem.value.length > 50) return false;
    return true;
  }

  public getOperatorDisplayName(searchItem: SearchItem) {
    if (searchItem.isValueSelectable) return 'Equal to';
    return this.getOperatorsByType(searchItem)?.find((_) => _.value === searchItem.operator)?.name;
  }

  public getValue(searchItem: SearchItem) {
    if (!searchItem || searchItem.operator === SearchOperators.IsNull || searchItem.operator === SearchOperators.IsNotNull) return;

    if (searchItem.type !== SearchTypes.MultiSelect)
      return searchItem.isValueSelectable
        ? `'${searchItem.listValues?.find((_) => _.value === searchItem.value)?.displayName ?? 'No value'}'`
        : `'${searchItem.value ?? 'No value'}'`;

    return `'${(<string[]>searchItem.value)?.join(', ') ?? 'No value'}'`;
  }

  public removeSearchItem(searchItem: SearchItem) {
    if (!searchItem) return;
    searchItem.selected = false;
    searchItem.operator = undefined;
    searchItem.value = undefined;
    this.onItemRemoved.emit(searchItem);
  }
}
