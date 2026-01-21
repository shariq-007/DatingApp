export type Member = {
  id: string
  dob: string
  imgURL?: string
  displayName: string
  created: string
  lastActive: string
  gender: string
  description?: string
  city: string
  country: string
}

export type Root = {
  id: number
  url: string
  publicId?: string
  memberId: string
}

