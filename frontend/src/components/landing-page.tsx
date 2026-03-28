import { ArrowRightIcon } from "lucide-react"

import { Button } from "@/components/ui/button"
import { useNavigate } from "react-router-dom"

const AboutUs = () => {
  const navigate = useNavigate()
  return (
    <section className="bg-muted py-8 sm:py-16 lg:py-24">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="mb-12 space-y-4 text-center md:mb-16 lg:mb-24">
          <h2 className="text-2xl font-semibold tracking-tight md:text-3xl lg:text-4xl">
            Manage Finances
          </h2>
          <p className="text-xl text-muted-foreground">
            Умно приложение за лични финанси, което ти помага да контролираш
            разходите си, да следиш приходите и да планираш бюджета си — всичко
            на едно място.
          </p>
          <Button
            size="lg"
            asChild
            className="group rounded-lg text-base has-[>svg]:px-6"
          >
            <a onClick={() => navigate("/finances")}>
              Продължи
              <ArrowRightIcon className="transition-transform duration-200 group-hover:translate-x-0.5" />
            </a>
          </Button>
        </div>

        <img
          src="https://cdn.shadcnstudio.com/ss-assets/blocks/marketing/about-us/image-44.png"
          alt="Илюстрация за нас"
          className="h-full w-full rounded-lg object-cover"
        />
      </div>
    </section>
  )
}

export default AboutUs
