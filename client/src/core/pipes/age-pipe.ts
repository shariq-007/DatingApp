import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'age',
})
export class AgePipe implements PipeTransform {

  transform(value: string): number {
    const today = new Date();
    const dob = new Date(value);

    let age = today.getFullYear() - dob.getFullYear();
    const monthdiff = today.getMonth() - dob.getMonth();

    if (monthdiff < 0 || (monthdiff === 0 && today.getDate() < dob.getDate())){
      age--;
    }
    return age;
  }
}
