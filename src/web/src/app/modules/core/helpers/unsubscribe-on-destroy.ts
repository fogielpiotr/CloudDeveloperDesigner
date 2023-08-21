import { Component, OnDestroy } from "@angular/core";
import { FormControl } from "@angular/forms";
import { Subject } from "rxjs";

@Component({
    template: ''
})
export class UnsubscribeOnDestroy implements OnDestroy {
    protected readonly destroy$ = new Subject<boolean>();

    ngOnDestroy(): void {
        this.destroy$.next(true);
        this.destroy$.complete();
    }
}