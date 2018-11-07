import { Injectable } from '@angular/core';
import { MemberEditComponent } from '../_components/members/member-edit/member-edit.component';
import { CanDeactivate } from '@angular/router';

@Injectable()
export class PreventUnsavedChanges
  implements CanDeactivate<MemberEditComponent> {
  canDeactivate(comp: MemberEditComponent) {
    if (comp.editForm.dirty) {
      return confirm(
        'Are you sure you want to continue?  Any unsaved changes will be lost'
      );
    }

    return true;
  }
}
